using System;
using System.Collections.Generic;
using System.Linq;
using CloudState.CSharpSupport.EventSourced;
using CloudState.CSharpSupport.Interfaces.EventSourced;
using CloudState.CSharpSupport.Interfaces.EventSourced.Contexts;
using CloudState.CSharpSupport.Reflection;
using CloudState.CSharpSupport.Reflection.Interfaces;
using CloudState.CSharpSupport.Serialization;
using CloudState.CSharpSupport.Tests.AttributeBasedEntityFactory.CommandHandler.Messages;
using CloudState.CSharpSupport.Tests.AttributeBasedEntityFactory.CommandHandler.ResolvedTypes;
using Google.Protobuf.WellKnownTypes;
using Moq;
using Optional.Unsafe;

namespace CloudState.CSharpSupport.Tests.AttributeBasedEntityFactory.CommandHandler
{
    public partial class AttributeBasedEntityFactoryCommandHandlerTests
    {
        public static IEventSourcedEntityHandler CreateHandler<T>(Func<IEventSourcedEntityCreationContext, object> entityFactory = null)
        {
            var anySupport = new AnySupport(
                new[] { Com.Example.Shoppingcart.Persistence.DomainReflection.Descriptor }
            );
            var mockSupport = new Mock<IEventSourcedContext>();
            mockSupport.Setup(x => x.EntityId)
                .Returns("foo");

            var method = new ResolvedServiceMethod<string, Wrapped>(
                Com.Example.Shoppingcart.ShoppingCart
                    .Descriptor.Methods
                    .First(x => x.Name == "AddItem"),
                new StringResolvedType(),
                new WrappedResolvedType()
            );

            return new AttributeBasedEntityHandlerFactory(
                typeof(T),
                anySupport,
                new[] { method }.ToDictionary(
                    x => x.Descriptor.Name,
                    x => x as IResolvedServiceMethod),
                entityFactory
            ).CreateEntityHandler(mockSupport.Object);
        }

        Wrapped DecodeWrapped(Optional.Option<Any> maybeAny)
        {
            return new WrappedResolvedType().ParseFrom(maybeAny.ValueOrFailure().Value);
        }

        private class MockCommandContextRef
        {
            public List<object> Emitted { get; }
            public ICommandContext Object { get; }
            public MockCommandContextRef(long cmdId = 1L, long seq = 1L, List<object> emitted = null)
            {
                Emitted = emitted ?? new List<object>();
                var context = new Mock<ICommandContext>();
                context.Setup(x => x.CommandName).Returns("AddItem");
                context.Setup(x => x.CommandId).Returns(cmdId);
                context.Setup(x => x.Sequence).Returns(seq);
                context.Setup(x => x.EntityId).Returns("foo");
                context.Setup(x => x.Emit(It.IsAny<object>()))
                    .Callback<object>(@event => Emitted.Add(@event));
                Object = context.Object;
            }
        }
    }
}