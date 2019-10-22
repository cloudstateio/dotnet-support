using System;
using System.Collections.Generic;
using Com.Example.Shoppingcart;
using Com.Example.Shoppingcart.Persistence;
using Google.Protobuf;
using Google.Protobuf.Reflection;
using Google.Protobuf.WellKnownTypes;
using io.cloudstate.csharpsupport.eventsourced;
using io.cloudstate.csharpsupport.impl;
using Moq;
using Xunit;

namespace csharp_support_tests
{
    public class CommandHandlerTests : EventSourcedAnnotationSupportTests
    {
        public class MockCommandContextRef
        {
            public List<object> Emitted { get; }
            public ICommandContext Context { get; }
            public MockCommandContextRef(long cmdId, long seq, string eid, List<object> emitted = null)
            {
                Emitted = emitted ?? new List<object>();
                var context = new Mock<ICommandContext>();
                context.Setup(x => x.CommandName).Returns("AddItem");
                context.Setup(x => x.CommandId).Returns(cmdId);
                context.Setup(x => x.SequenceNumber).Returns(seq);
                context.Setup(x => x.EntityId).Returns(eid);
                context.Setup(x => x.Emit(It.IsAny<object>()))
                    .Callback<object>(@event => Emitted.Add(@event));
                Context = context.Object;
            }
        }

        static ICommandContext MockCommandContext(long cmdId, long seq, string eid, List<object> emitted = null)
        {
            return new MockCommandContextRef(cmdId, seq, eid, emitted).Context;
        }

        // NOTE: Original tests had this returning an 'Any', but the original
        // spec returns 'Empty'.  Maybe it's because of the mock context that 
        // I didn't do correctly??
        //    - Assert -> decodeWrapped()should ===(Wrapped("blah")

        [Theory]
        [InlineData(10L, 20L, "eid")]
        [InlineData(15L, 25L, "id")]
        public void NoArgCommandHandlerTest(long cmdId, long seq, string eid)
        {
            NoArgCommandHandlerEntity entity = null;
            var handler = Create<NoArgCommandHandlerEntity>(
                (ctx) => entity = new NoArgCommandHandlerEntity(ctx)
            );
            var result = handler.HandleCommand(
                Command("command-data"),
                MockCommandContext(cmdId, seq, eid)
            );

            result.Match(
                some: x => Assert.Equal(x.Value, new Empty().ToByteString()),
                none: () => throw new Exception("Failed")
            );

            Assert.True(entity.Invoked);
        }


        [Theory]
        [InlineData(10L, 20L, "eid")]
        [InlineData(15L, 25L, "id")]
        public void SingleArgCommandHandlerTest(long cmdId, long seq, string eid)
        {
            SingleArgCommandHandlerEntity entity = null;
            var handler = Create<SingleArgCommandHandlerEntity>(
                (ctx) => entity = new SingleArgCommandHandlerEntity(ctx)
            );
            var result = handler.HandleCommand(
                Command("command-data"),
                MockCommandContext(cmdId, seq, eid)
            );

            result.Match(
                some: x => Assert.Equal(x.Value, new Empty().ToByteString()),
                none: () => throw new Exception("Failed")
            );

            Assert.True(entity.Invoked);
        }

        [Theory]
        [InlineData(10L, 20L, "eid")]
        [InlineData(15L, 25L, "id")]
        public void MultiArgCommandHandlerTest(long cmdId, long seq, string eid)
        {
            MultiArgCommandHandlerEntity entity = null;
            var handler = Create<MultiArgCommandHandlerEntity>(
                (ctx) => entity = new MultiArgCommandHandlerEntity(ctx, cmdId, seq, eid)
            );
            var result = handler.HandleCommand(
                Command("command-data"),
                MockCommandContext(cmdId, seq, eid)
            );

            result.Match(
                some: x => Assert.Equal(x.Value, new Empty().ToByteString()),
                none: () => throw new Exception("Failed")
            );

            Assert.True(entity.Invoked);
        }

        [Theory]
        [InlineData(10L, 20L, "eid", "carrots", "00001", 15)]
        [InlineData(15L, 25L, "id", "apples", "00002", 12)]
        public void AllowEmittingEvents(long cmdId, long seq, string eid, string name, string id, int quantity)
        {
            List<object> emitted = new List<object>();
            AllowEmittingEventsEntity entity = null;
            var handler = Create<AllowEmittingEventsEntity>(
                (ctx) => entity = new AllowEmittingEventsEntity(ctx)
            );
            var context = MockCommandContext(cmdId, seq, eid, emitted);
            var result = handler.HandleCommand(
                Any.Pack(new AddLineItem()
                {
                    Name = name,
                    ProductId = id,
                    Quantity = quantity
                }),
                context
            );

            result.Match(
                some: x => Assert.Equal(x.Value, new Empty().ToByteString()),
                none: () => throw new Exception("Failed")
            );

            Assert.True(entity.Invoked);

            var @event = new Com.Example.Shoppingcart.Persistence.ItemAdded()
            {
                Item = new Com.Example.Shoppingcart.Persistence.LineItem()
                {
                    Name = name,
                    ProductId = id,
                    Quantity = quantity
                }
            };

            Assert.Single(emitted);
            Assert.Equal(emitted[0], @event);

        }


        /*
        

      "fail if there's two command handlers for the same command" in {
        a[RuntimeException] should be thrownBy create(new {
          @CommandHandler
          def addItem(msg: String, ctx: CommandContext) =
            Wrapped(msg)
          @CommandHandler
          def addItem(msg: String) =
            Wrapped(msg)
        }, method)
      }

      "fail if there's no command with that name" in {
        a[RuntimeException] should be thrownBy create(new {
          @CommandHandler
          def wrongName(msg: String) =
            Wrapped(msg)
        }, method)
      }

      "fail if there's a CRDT command handler" in {
        val ex = the[RuntimeException] thrownBy create(new {
            @io.cloudstate.javasupport.crdt.CommandHandler
            def addItem(msg: String) =
              Wrapped(msg)
          }, method)
        ex.getMessage should include("Did you mean")
        ex.getMessage should include(classOf[CommandHandler].getName)
      }

      "unwrap exceptions" in {
        val handler = create(new {
          @CommandHandler
          def addItem(): Wrapped = throw new RuntimeException("foo")
        }, method)
        val ex = the[RuntimeException] thrownBy handler.handleCommand(command("nothing"), new MockCommandContext)
        ex.getMessage should ===("foo")
      }
    */


        Any Command(object obj)
        {
            var anySupport = new AnySupport(
                new FileDescriptor[] { }
            );
            return anySupport.Encode(obj);
        }

        [EventSourcedEntity]
        public class NoArgCommandHandlerEntity
        {
            IEventSourcedContext Ctx { get; }
            public bool Invoked { get; private set; }

            public NoArgCommandHandlerEntity(IEventSourcedContext ctx)
            {
                Ctx = ctx;
            }

            [CommandHandler]
            public Empty AddItem()
            {
                Invoked = true;
                return new Empty();
            }
        }

        [EventSourcedEntity]
        public class SingleArgCommandHandlerEntity
        {
            IEventSourcedContext Ctx { get; }
            public bool Invoked { get; private set; }

            public SingleArgCommandHandlerEntity(IEventSourcedContext ctx)
            {
                Ctx = ctx;
            }

            [CommandHandler]
            public Empty AddItem(AddLineItem command)
            {
                Invoked = true;
                return new Empty();
            }
        }

        [EventSourcedEntity]
        public class MultiArgCommandHandlerEntity
        {
            IEventSourcedContext Ctx { get; }
            public long CmdId { get; }
            public long Seq { get; }
            public string Eid { get; }
            public bool Invoked { get; private set; }

            public MultiArgCommandHandlerEntity(IEventSourcedContext ctx, long cmdId, long seq, string eid)
            {
                Ctx = ctx;
                CmdId = cmdId;
                Seq = seq;
                Eid = eid;
            }

            [CommandHandler]
            public Empty AddItem(AddLineItem command, [EntityId]string entityId, ICommandContext commandContext)
            {
                Invoked = true;
                Assert.Equal(Eid, entityId);
                Assert.Equal(CmdId, commandContext.CommandId);
                Assert.Equal(Seq, commandContext.SequenceNumber);
                Assert.Equal("AddItem", commandContext.CommandName);
                return new Empty();
            }
        }

        [EventSourcedEntity]
        public class AllowEmittingEventsEntity
        {
            IEventSourcedContext Ctx { get; }
            public bool Invoked { get; private set; }

            public AllowEmittingEventsEntity(IEventSourcedContext ctx)
            {
                Ctx = ctx;
            }

            [CommandHandler]
            public Empty AddItem(AddLineItem command, ICommandContext context)
            {
                Invoked = true;
                context.Emit(new ItemAdded()
                {
                    Item = new Com.Example.Shoppingcart.Persistence.LineItem()
                    {
                        Name = command.Name,
                        ProductId = command.ProductId,
                        Quantity = command.Quantity
                    }
                });
                return new Empty();
            }
        }
    }
}
