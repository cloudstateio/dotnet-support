using System;
using System.Collections.Generic;
using Google.Protobuf.WellKnownTypes;
using io.cloudstate.csharpsupport.eventsourced;
using io.cloudstate.csharpsupport.impl;
using io.cloudstate.csharpsupport.impl.eventsourced;
using Moq;
using Xunit;
using static io.cloudstate.csharpsupport.impl.AnySupport;

namespace csharp_support_tests
{
    public class EventHandlerTests : EventSourcedAnnotationSupportTests
    {
        [Fact]
        public void NoArgEventHandlerTest()
        {
            var eventContext = new EventContext("", 0);
            var entity = new NoArgEventHandlerEntity();
            var entityHandler = Create<NoArgEventHandlerEntity>(x => entity);
            entityHandler.HandleEvent(Event(""), eventContext);
            Assert.True(entity.Invoked);
        }

        [Theory]
        [InlineData("foo")]
        [InlineData("event-data")]
        public void SingleArgEventHandlerTest(string eventData)
        {
            var eventContext = new EventContext("", 0);
            var entity = new SingleArgEventHandlerEntity();
            var entityHandler = Create<SingleArgEventHandlerEntity>(x => entity);
            entityHandler.HandleEvent(Event(eventData), eventContext);
            Assert.True(entity.Invoked);
            Assert.Equal(eventData, entity.Event);
        }

        Any Event(object obj)
        {
            var anySupport = new AnySupport(
                new[] { Com.Example.Shoppingcart.Persistence.DomainReflection.Descriptor }
            );
            return anySupport.Encode(obj);
        }

        [Theory]
        [InlineData("id", "message", 20L)]
        [InlineData("eid", "entity-data", 10L)]
        public void MultiArgEventHandlerTest(string entityId, string eventData, long sequenceNumber)
        {
            var eventContext = new EventContext(entityId, sequenceNumber);
            var entity = new MultiArgEventHandlerEntity();
            var entityHandler = Create<MultiArgEventHandlerEntity>(x => entity);

            entityHandler.HandleEvent(Event(eventData), eventContext);

            Assert.True(entity.Invoked);
            Assert.Equal(eventData, entity.Event);
            Assert.Equal(sequenceNumber, entity.SequenceNumber);
        }

        [Theory]
        [InlineData("foo")]
        [InlineData("event-data")]
        public void SubclassedArgEventHandlerEntityTest(string eventData)
        {
            var eventContext = new EventContext("", 0);
            var entity = new SubclassedArgEventHandlerEntity();
            var entityHandler = Create<SubclassedArgEventHandlerEntity>(x => entity);
            entityHandler.HandleEvent(Event(eventData), eventContext);
            Assert.True(entity.Invoked);
            Assert.Equal(eventData, entity.Event);
        }

        [Theory]
        [InlineData("foo")]
        [InlineData("event-data")]
        public void SubinterfacedArgEventHandlerEntityTest(string eventData)
        {
            var eventContext = new EventContext("", 0);
            var entity = new SubinterfacedArgEventHandlerEntity();
            var entityHandler = Create<SubinterfacedArgEventHandlerEntity>(x => entity);
            entityHandler.HandleEvent(Event(eventData), eventContext);
            Assert.True(entity.Invoked);
            Assert.Equal(eventData, entity.Event);
        }

        [Fact(Skip = "Not ready")]
        public void AllowChangingBehavior()
        {

        }

        [Fact(Skip = "Not ready")]
        public void FailOnEventHandlerClassConflict()
        {
            /*
            "fail if the event handler class conflicts with the event class" in {
                a[RuntimeException] should be thrownBy create(new {
                @EventHandler(eventClass = classOf[Integer])
                def handle(event: String) = ()
                })
            } 
            */
        }

        [Fact(Skip = "Not ready")]
        public void FailOnMultipleEventHandlersOfSameType()
        {
            /*
            "fail if there are two event handlers for the same type" in {
                a[RuntimeException] should be thrownBy create(new {
                  @EventHandler
                  def handle1(event: String) = ()

                  @EventHandler
                  def handle2(event: String) = ()
                })
              }
            */
        }


        [Fact(Skip = "Not ready")]
        public void FailIfEntityIdAnnotatedParameterIsNotString()
        {
            //   "fail if an EntityId annotated parameter is not a string" in {
            //     a[RuntimeException] should be thrownBy create(new {
            //       @EventHandler
            //       def handle(event: String, @EntityId entityId: Int) = ()
            //     })
            //   }
        }

        [EventSourcedEntity]
        public class NoArgEventHandlerEntity
        {
            public bool Invoked { get; private set; }

            [EventHandler(typeof(String))]
            public void OnEvent()
            {
                Invoked = true;
            }
        }

        [EventSourcedEntity]
        public class SingleArgEventHandlerEntity
        {
            public bool Invoked { get; private set; }
            public string Event { get; private set; }

            [EventHandler(typeof(String))]
            public void OnEvent(String @event)
            {
                Invoked = true;
                Event = @event;
            }
        }

        [EventSourcedEntity]
        public class MultiArgEventHandlerEntity
        {
            public string EntityId { get; private set; }
            public string Event { get; private set; }
            public bool Invoked { get; private set; }
            public long SequenceNumber { get; private set; }

            [EventHandler(typeof(String))]
            public void OnEvent([EntityId]string entityId, String @event, IEventContext context)
            {
                EntityId = entityId;
                Event = @event;
                Invoked = true;
                SequenceNumber = context.SequenceNumber;
                Assert.Equal(EntityId, entityId);
            }
        }

        [EventSourcedEntity]
        public class SubclassedArgEventHandlerEntity
        {
            public bool Invoked { get; private set; }
            public Object Event { get; private set; }

            [EventHandler]
            public void OnEvent(Object @event)
            {
                Invoked = true;
                Event = @event;
            }
        }

        [EventSourcedEntity]
        public class SubinterfacedArgEventHandlerEntity
        {
            public bool Invoked { get; private set; }
            public IEnumerable<char> Event { get; private set; }

            [EventHandler]
            public void OnEvent(IEnumerable<char> @event)
            {
                Invoked = true;
                Event = @event;
            }
        }
    }
}
