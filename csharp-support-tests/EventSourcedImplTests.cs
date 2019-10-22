using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudstate.Eventsourced;
using Com.Example.Shoppingcart;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using io.cloudstate.csharpsupport;
using io.cloudstate.csharpsupport.eventsourced;
using io.cloudstate.csharpsupport.eventsourced.impl;
using io.cloudstate.csharpsupport.impl;
using io.cloudstate.csharpsupport.impl.eventsourced;
using io.cloudstate.samples.shoppingCart;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace csharp_support_tests
{
    public class EventSourcedImplTests
    {
        public static EventSourcedService SetupEventSourcedImpl<T>(
            Google.Protobuf.Reflection.ServiceDescriptor descriptor,
            params Google.Protobuf.Reflection.FileDescriptor[] additionalDescriptors
        )
        {
            var anySupport = new AnySupport(additionalDescriptors);
            var entity = typeof(T).GetCustomAttributes(
                    typeof(EventSourcedEntityAttribute),
                    true
                ).FirstOrDefault() as EventSourcedEntityAttribute;
            var services = new Dictionary<string, IStatefulService>();
            services.Add(
                descriptor.FullName,
                new EventSourcedStatefulService(
                    new AnnotationBasedEventSourcedSupport<T>(anySupport, descriptor),
                    descriptor,
                    anySupport,
                    entity.PersistenceId,
                    entity.SnapshotEvery
                )
            );
            var rootContext = new Context(new ResolvedServiceCallFactory(services));

            var impl = new EventSourcedService(
                new LoggerFactory(),
                new Configuration(new ConfigurationBuilder().Build()),
                services,
                rootContext
            );

            return impl;

        }

        [Fact]
        public async Task CanInstantiateAndSendCommands()
        {
            var impl = SetupEventSourcedImpl<ShoppingCartEntity>(
                Com.Example.Shoppingcart.ShoppingCart.Descriptor,
                Com.Example.Shoppingcart.Persistence.DomainReflection.Descriptor
            );

            var inputs = new[] {
                new EventSourcedStreamIn() {
                    Init = new EventSourcedInit() {
                        EntityId = "100",
                        ServiceName = "com.example.shoppingcart.ShoppingCart"
                    }
                },
                new EventSourcedStreamIn() {
                    Command = new Cloudstate.Command() {
                        EntityId = "100",
                        Id = 500,
                        Name = "AddItem",
                        Payload = Any.Pack(new AddLineItem() {
                            Name = "Apple",
                            ProductId = "0001",
                            Quantity = 100,
                            UserId = "user_0001"
                        })
                    }
                },
                new EventSourcedStreamIn() {
                    Command = new Cloudstate.Command() {
                        EntityId = "100",
                        Id = 501,
                        Name = "GetCart"
                    }
                }
            };

            var calls = 0;
            var input = new Mock<IAsyncStreamReader<EventSourcedStreamIn>>();
            var output = new Mock<IServerStreamWriter<EventSourcedStreamOut>>();
            input.Setup(x => x.MoveNext(It.IsAny<CancellationToken>()))
                .Returns(() =>
                {
                    calls = calls + 1;
                    return Task.FromResult(calls <= inputs.Length);
                });
            input.Setup(x => x.Current)
                .Returns(() => inputs[calls - 1]);
            output.Setup(x => x.WriteAsync(It.IsAny<EventSourcedStreamOut>()))
                .Callback<EventSourcedStreamOut>(o =>
                {
                    Assert.NotNull(o.Reply);
                    Assert.NotNull(o.Reply.Events);
                    Assert.NotEqual(0L, o.Reply.CommandId);
                });

            await impl.handle(input.Object, output.Object, null);



        }
    }
}
