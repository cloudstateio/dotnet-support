using System;
using Com.Example.Shoppingcart.Persistence;
using io.cloudstate.csharpsupport.eventsourced;
using io.cloudstate.csharpsupport.impl;
using io.cloudstate.csharpsupport.impl.eventsourced;
using Moq;
using Xunit;

namespace csharp_support_tests
{
    public partial class EventSourcedAnnotationSupportTests
    {

        public static IEventSourcedEntityHandler Create<T>(Func<IEventSourcedContext, T> entityFactory = null)
        {
            var anySupport = new AnySupport(
                new[] { Com.Example.Shoppingcart.Persistence.DomainReflection.Descriptor }
            );
            var mockSupport = new Mock<IEventSourcedContext>();
            mockSupport.Setup(x => x.EntityId)
                .Returns("foo");
            return new AnnotationBasedEventSourcedSupport<T>(
                    anySupport,
                    Com.Example.Shoppingcart.ShoppingCart.Descriptor,
                    entityFactory
                )
                .Create(mockSupport.Object);
        }

    }
}
