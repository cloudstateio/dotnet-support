// using System.Collections.Generic;
// using System.Linq;
// using System.Reflection;
// using System.Threading;
// using System.Threading.Tasks;
// using CloudState.CSharpSupport.Attributes.EventSourced;
// using CloudState.CSharpSupport.Contexts;
// using CloudState.CSharpSupport.EventSourced;
// using CloudState.CSharpSupport.EventSourced.Interfaces;
// using CloudState.CSharpSupport.EventSourced.Services;
// using CloudState.CSharpSupport.Interfaces;
// using CloudState.CSharpSupport.Reflection;
// using CloudState.CSharpSupport.Serialization;
// using Cloudstate.Eventsourced;
// using Com.Example.Shoppingcart;
// using EventSourced.ShoppingCart;
// using Google.Protobuf.WellKnownTypes;
// using Grpc.Core;
// using Microsoft.Extensions.Configuration;
// using Microsoft.Extensions.Logging;
// using Moq;
// using Xunit;

// namespace CloudState.CSharpSupport.Tests
// {
//     public class EventSourcedImplementationTests
//     {
//         static EntityCollectionService SetupEventSourcedImpl<T>(
//             Google.Protobuf.Reflection.ServiceDescriptor descriptor,
//             params Google.Protobuf.Reflection.FileDescriptor[] additionalDescriptors
//         )
//         {
//             var anySupport = new AnySupport(additionalDescriptors);
//             var entity = typeof(T).GetCustomAttribute<EventSourcedEntityAttribute>(true);
//             IReadOnlyDictionary<string, IEventSourcedStatefulService> services = new Dictionary<string, IEventSourcedStatefulService>
//             {
//                 {
//                     descriptor.FullName, new EventSourcedStatefulService(
//                         new AttributeBasedEntityHandlerFactory(
//                             typeof(T),
//                             anySupport,
//                             descriptor
//                         ),
//                         descriptor,
//                         anySupport,
//                         entity.PersistenceId,
//                         entity.SnapshotEvery
//                     )
//                 }
//             };
//             var rootContext = new Context(new ResolvedServiceCallFactory(services.ToDictionary(x => x.Key, x => x.Value as IStatefulService)));

//             var impl = new EntityCollectionService(
//                 LoggerFactory.Create(x => x.AddConsole()),
//                 new CloudStateWorker.CloudStateConfiguration(
//                     new Mock<IConfiguration>().Object),
//                 services,
//                 rootContext
//             );

//             return impl;

//         }

//         [Fact]
//         public async Task CanInstantiateAndSendCommands()
//         {
//             var impl = SetupEventSourcedImpl<ShoppingCartEntity>(
//                 ShoppingCart.Descriptor,
//                 Com.Example.Shoppingcart.Persistence.DomainReflection.Descriptor
//             );

//             var inputs = new[] {
//                 new EventSourcedStreamIn() {
//                     Init = new EventSourcedInit() {
//                         EntityId = "100",
//                         ServiceName = "com.example.shoppingcart.ShoppingCart"
//                     }
//                 },
//                 new EventSourcedStreamIn() {
//                     Command = new Cloudstate.Command() {
//                         EntityId = "100",
//                         Id = 500,
//                         Name = "AddItem",
//                         Payload = Any.Pack(new AddLineItem() {
//                             Name = "Apple",
//                             ProductId = "0001",
//                             Quantity = 100,
//                             UserId = "user_0001"
//                         })
//                     }
//                 },
//                 new EventSourcedStreamIn() {
//                     Command = new Cloudstate.Command() {
//                         EntityId = "100",
//                         Id = 501,
//                         Name = "GetCart"
//                     }
//                 }
//             };

//             var calls = 0;
//             var input = new Mock<IAsyncStreamReader<EventSourcedStreamIn>>();
//             var output = new Mock<IServerStreamWriter<EventSourcedStreamOut>>();
//             input.Setup(x => x.MoveNext(It.IsAny<CancellationToken>()))
//                 .Returns(() =>
//                 {
//                     calls = calls + 1;
//                     return Task.FromResult(calls <= inputs.Length);
//                 });
//             input.Setup(x => x.Current)
//                 .Returns(() => inputs[calls - 1]);
//             var called = false;
//             output.Setup(x => x.WriteAsync(It.IsAny<EventSourcedStreamOut>()))
//                 .Callback<EventSourcedStreamOut>(o =>
//                 {
//                     called = true;
//                     Assert.NotNull(o.Reply);
//                     Assert.NotNull(o.Reply.Events);
//                     Assert.NotEqual(0L, o.Reply.CommandId);
//                 });

//             await impl.handle(input.Object, output.Object, null);
            
//             Assert.True(called);

//         }
//     }
// }