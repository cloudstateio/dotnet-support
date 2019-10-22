using Xunit;

namespace csharp_support_tests
{
    public class SnapshotTests : EventSourcedAnnotationSupportTests
    {
        [Fact(Skip = "Not ready")]
        public void TestName()
        {

        }
        /*
        "support snapshots" when {
      val ctx = new SnapshotContext with BaseContext {
        override def sequenceNumber(): Long = 10
        override def entityId(): String = "foo"
      }

      "no arg parameter" in {
        val handler = create(new {
          @Snapshot
          def createSnapshot: String = "snap!"
        })
        val snapshot = handler.snapshot(ctx)
        snapshot.isPresent shouldBe true
        anySupport.decode(snapshot.get) should ===("snap!")
      }

      "context parameter" in {
        val handler = create(new {
          @Snapshot
          def createSnapshot(ctx: SnapshotContext): String = {
            ctx.entityId() should ===("foo")
            "snap!"
          }
        })
        val snapshot = handler.snapshot(ctx)
        snapshot.isPresent shouldBe true
        anySupport.decode(snapshot.get) should ===("snap!")
      }

      "fail if there's two snapshot methods" in {
        a[RuntimeException] should be thrownBy create(new {
          @Snapshot
          def createSnapshot1: String = "snap!"
          @Snapshot
          def createSnapshot2: String = "snap!"
        })
      }

      "fail if there's a bad context" in {
        a[RuntimeException] should be thrownBy create(new {
          @Snapshot
          def createSnapshot(context: EventContext): String = "snap!"
        })
      }

    } */
    }
}
