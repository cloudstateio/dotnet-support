using System;
using CloudState.CSharpSupport.Interfaces.EventSourced;
using CloudState.CSharpSupport.Interfaces.EventSourced.Contexts;
using CloudState.CSharpSupport.Tests.AttributeBasedEntityFactory.EventHandler.Messages;
using CloudState.CSharpSupport.Tests.AttributeBasedEntityFactory.SnapshotHandler.Entities;
using Xunit;

namespace CloudState.CSharpSupport.Tests.AttributeBasedEntityFactory.SnapshotHandler
{
    public class SnapshotHandlerTests
    {
        [Fact]
        public void can_instantiate_entity_handler()
        {
            var handler = CreateHandler<SingleArgSnapshotHandlerEntity>();
            handler.HandleSnapshot(Event.Create("snap!"), new SnapshotHandlerTestsHelper.MockSnapshotContextRef().Object);
            Assert.NotNull(handler);
        }

        [Fact]
        public void supports_single_arg_snapshot_handler()
        {
            var obj = new SingleArgSnapshotHandlerEntity();
            var handler = CreateHandler<SingleArgSnapshotHandlerEntity>(x => obj);
            handler.HandleSnapshot(Event.Create("snap!"), new SnapshotHandlerTestsHelper.MockSnapshotContextRef().Object);
            Assert.True(obj.Invoked);
        }

        private IEventSourcedEntityHandler CreateHandler<T>(Func<IEventSourcedEntityCreationContext, object> entityFactory = null) => SnapshotHandlerTestsHelper.CreateHandler<T>(entityFactory);

    }
}