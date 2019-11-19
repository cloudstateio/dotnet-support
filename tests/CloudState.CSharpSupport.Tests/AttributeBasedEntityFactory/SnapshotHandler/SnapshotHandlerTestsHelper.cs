using System;
using System.Linq;
using CloudState.CSharpSupport.EventSourced;
using CloudState.CSharpSupport.Interfaces.EventSourced;
using CloudState.CSharpSupport.Interfaces.EventSourced.Contexts;
using CloudState.CSharpSupport.Reflection.Interfaces;
using CloudState.CSharpSupport.Serialization;
using Moq;

namespace CloudState.CSharpSupport.Tests.AttributeBasedEntityFactory.SnapshotHandler
{
    public static class SnapshotHandlerTestsHelper
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

        internal class MockSnapshotContextRef
        {
            public ISnapshotContext Object { get; }
            public MockSnapshotContextRef(long seq = 1L)
            {
                var context = new Mock<ISnapshotContext>();
                context.Setup(x => x.SequenceNumber).Returns(seq);
                context.Setup(x => x.EntityId).Returns("foo");
                Object = context.Object;
            }
        }
    }
}