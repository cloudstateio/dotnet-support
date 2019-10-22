using Xunit;

namespace csharp_support_tests
{
    public class SnapshotHandlerTests : EventSourcedAnnotationSupportTests
    {
        [Fact(Skip = "Not ready")]
        public void TestName()
        {

        }

        /*
        
        "support snapshot handlers" when {
      val ctx = new SnapshotContext with BaseContext {
        override def sequenceNumber(): Long = 10
        override def entityId(): String = "foo"
      }

      "single parameter" in {
        var invoked = false
        val handler = create(new {
          @SnapshotHandler
          def handleSnapshot(snapshot: String) = {
            snapshot should ===("snap!")
            invoked = true
          }
        })
        handler.handleSnapshot(event("snap!"), ctx)
        invoked shouldBe true
      }

      "context parameter" in {
        var invoked = false
        val handler = create(new {
          @SnapshotHandler
          def handleSnapshot(snapshot: String, context: SnapshotBehaviorContext) = {
            snapshot should ===("snap!")
            context.sequenceNumber() should ===(10)
            invoked = true
          }
        })
        handler.handleSnapshot(event("snap!"), ctx)
        invoked shouldBe true
      }

      "changing behavior" in {
        var invoked = false
        var invoked2 = false
        val handler = create(new {
          @SnapshotHandler
          def handleSnapshot(snapshot: String, context: SnapshotBehaviorContext) = {
            snapshot should ===("snap!")
            context.sequenceNumber() should ===(10)
            context.become(new {
              @EventHandler
              def handleEvent(event: String) = {
                event should ===("my-event")
                invoked2 = true
              }
            })
            invoked = true
          }
        })
        handler.handleSnapshot(event("snap!"), ctx)
        invoked shouldBe true
        handler.handleEvent(event("my-event"), eventCtx)
        invoked2 shouldBe true
      }

      "fail if there's a bad context" in {
        a[RuntimeException] should be thrownBy create(new {
          @SnapshotHandler
          def handleSnapshot(snapshot: String, context: EventContext) = ()
        })
      }

      "fail if there's no snapshot parameter" in {
        a[RuntimeException] should be thrownBy create(new {
          @SnapshotHandler
          def handleSnapshot(context: SnapshotContext) = ()
        })
      }

      "fail if there's no snapshot handler for the given type" in {
        val handler = create(new {
          @SnapshotHandler
          def handleSnapshot(snapshot: Int) = ()
        })
        a[RuntimeException] should be thrownBy handler.handleSnapshot(event(10), ctx)
      }
       */
    }
}
