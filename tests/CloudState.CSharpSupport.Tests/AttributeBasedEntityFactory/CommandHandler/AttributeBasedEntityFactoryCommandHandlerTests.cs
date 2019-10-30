using System;
using CloudState.CSharpSupport.EventSourced;
using CloudState.CSharpSupport.Tests.AttributeBasedEntityFactory.CommandHandler.Entities;
using CloudState.CSharpSupport.Tests.AttributeBasedEntityFactory.CommandHandler.Messages;
using Xunit;

namespace CloudState.CSharpSupport.Tests.AttributeBasedEntityFactory.CommandHandler
{
    public partial class AttributeBasedEntityFactoryCommandHandlerTests
    {
        [Fact]
        public void can_instantiate_entity_handler()
        {
            var handler = CreateHandler<NoArgCommandHandlerEntity>();
            Assert.NotNull(handler);
        }
        
        [Fact]
        public void supports_no_arg_command_handler()
        {
            var handler = CreateHandler<NoArgCommandHandlerEntity>();
            Assert.Equal(
                new Wrapped("blah"),
                DecodeWrapped(handler.HandleCommand(Command.Create("nothing"), new MockCommandContextRef().Object))
            );
        }
        
        [Fact]
        public void supports_single_arg_command_handler()
        {
            var handler = CreateHandler<SingleArgCommandHandlerEntity>();
            Assert.Equal(
                new Wrapped("blah"),
                DecodeWrapped(handler.HandleCommand(Command.Create("blah"), new MockCommandContextRef().Object))
            );
        }

        [Fact]
        public void supports_multi_arg_command_handler()
        {
            var handler = CreateHandler<MultiArgCommandHandlerEntity>();
            Assert.Equal(
                new Wrapped("blah"),
                DecodeWrapped(handler.HandleCommand(Command.Create("blah"), new MockCommandContextRef().Object))
            );
        }
        
        [Fact]
        public void should_allow_emitting_events()
        {
            var handler = CreateHandler<AllowEmittingEventsEntity>();
            var ctx = new MockCommandContextRef();
            Assert.Equal(
                new Wrapped("blah"),
                DecodeWrapped(handler.HandleCommand(Command.Create("blah"), ctx.Object))
            );
            Assert.Equal("blah event", ctx.Emitted[0]);
        }

        [Fact]
        public void should_fail_on_bad_context_type()
        {
            var ex = Assert.Throws<CloudStateException>(() =>
            {
                var handler = CreateHandler<BadContextTypeEntity>();
                var ctx = new MockCommandContextRef();
                Assert.Equal(
                    new Wrapped("blah"),
                    DecodeWrapped(handler.HandleCommand(Command.Create("blah"), ctx.Object))
                );
                Assert.Equal("blah event", ctx.Emitted[0]);
            });
            Assert.StartsWith("Unsupported context parameter on [AddItem]", ex.Message);

        }

        

    }
}
