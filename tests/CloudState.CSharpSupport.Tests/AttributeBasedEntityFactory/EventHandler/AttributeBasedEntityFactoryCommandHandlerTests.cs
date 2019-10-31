using CloudState.CSharpSupport.Exceptions;
using CloudState.CSharpSupport.Tests.AttributeBasedEntityFactory.CommandHandler.Entities;
using CloudState.CSharpSupport.Tests.AttributeBasedEntityFactory.CommandHandler.Messages;
using CloudState.CSharpSupport.Tests.AttributeBasedEntityFactory.EventHandler.Entities;
using CloudState.CSharpSupport.Tests.AttributeBasedEntityFactory.EventHandler.Messages;
using Xunit;

namespace CloudState.CSharpSupport.Tests.AttributeBasedEntityFactory.EventHandler
{
    public partial class AttributeBasedEntityFactoryEventHandlerTests
    {
        [Fact]
        public void can_instantiate_entity_handler()
        {
            var handler = CreateHandler<NoArgEventHandlerEntity>();
            Assert.NotNull(handler);
        }

        [Fact]
        public void supports_no_arg_event_handler()
        {
            var obj = new NoArgEventHandlerEntity();
            var handler = CreateHandler<NoArgEventHandlerEntity>(x => obj);
            handler.HandleEvent(Event.Create("nothing"), new MockEventContextRef().Object);
            Assert.True(obj.Invoked);
        }

        


    }
}
