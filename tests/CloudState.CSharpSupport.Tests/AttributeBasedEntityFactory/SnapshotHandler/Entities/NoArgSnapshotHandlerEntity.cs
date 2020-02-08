using CloudState.CSharpSupport.Attributes.EventSourced;
using Xunit;

namespace CloudState.CSharpSupport.Tests.AttributeBasedEntityFactory.SnapshotHandler.Entities
{
    [EventSourcedEntity]
    public class SingleArgSnapshotHandlerEntity
    {
        public bool Invoked { get; private set; }

        [SnapshotHandler]
        public void HandleSnapshot(string snapshot)
        {
            Assert.Equal("snap!", snapshot);
            Invoked = true;
        }
    }

    /*
     * val ctx = new SnapshotContext with BaseContext {
        override def sequenceNumber(): Long = 10
        override def entityId(): String = "foo"
      }

      "single parameter" in {
        var invoked = false
        handler.handleSnapshot(event("snap!"), ctx)
        invoked shouldBe true
      }
     */
}