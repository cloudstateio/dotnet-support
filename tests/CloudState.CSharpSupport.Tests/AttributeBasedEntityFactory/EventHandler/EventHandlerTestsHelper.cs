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

namespace CloudState.CSharpSupport.Tests.AttributeBasedEntityFactory.EventHandler
{
    public static class EventHandlerTestsHelper
    {
        internal static IEventSourcedEntityHandler CreateHandler<T>(Func<IEventSourcedEntityCreationContext, object> entityFactory = null)
        {
            var anySupport = new AnySupport(
                new[] { Com.Example.Shoppingcart.Persistence.DomainReflection.Descriptor }
            );
            var mockSupport = new Mock<IEventSourcedContext>();
            mockSupport.Setup(x => x.EntityId)
                .Returns("foo");

            return new AttributeBasedEntityHandlerFactory(
                typeof(T),
                anySupport,
                new IResolvedServiceMethod[] { }.ToDictionary(
                    x => x.Descriptor.Name,
                    x => x),
                entityFactory
            ).CreateEntityHandler(mockSupport.Object);
        }

        internal class MockEventContextRef
        {
            public IEventContext Object { get; }
            public MockEventContextRef(long seq = 1L)
            {
                var context = new Mock<IEventContext>();
                context.Setup(x => x.SequenceNumber).Returns(seq);
                context.Setup(x => x.EntityId).Returns("foo");
                Object = context.Object;
            }
        }
    }
}