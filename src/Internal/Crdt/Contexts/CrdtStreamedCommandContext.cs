using Cloudstate;

namespace CloudState.CSharpSupport.Crdt.Contexts
{
    internal class CrdtStreamedCommandContext : CrdtCommandContext // with IStreamedCommandContext<object> 
    {
        public CrdtStreamedCommandContext(Command command) :base(command)
        {
            
        }
        
        /*
         * private final var changeCallback: Option[function.Function[SubscriptionContext, Optional[JavaPbAny]]] = None
      private final var cancelCallback: Option[Consumer[StreamCancelledContext]] = None

      override final def isStreamed: Boolean = command.streamed

      override final def onChange(subscriber: function.Function[SubscriptionContext, Optional[JavaPbAny]]): Unit = {
        checkActive()
        changeCallback = Some(subscriber)
      }

      override final def onCancel(effect: Consumer[StreamCancelledContext]): Unit = {
        checkActive()
        cancelCallback = Some(effect)
      }

      final def addCallbacks(): Boolean = {
        changeCallback.foreach { onChange =>
          subscribers = subscribers.updated(command.id, onChange)
        }
        cancelCallback.foreach { onCancel =>
          cancelListeners = cancelListeners.updated(command.id, onCancel)
        }
        changeCallback.isDefined || cancelCallback.isDefined
      }
         */
    }
}