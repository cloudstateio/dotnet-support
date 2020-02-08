using System;
using System.Collections.Generic;
using Cloudstate;
using Cloudstate.Crdt;
using CloudState.CSharpSupport.Crdt.Contexts;
using CloudState.CSharpSupport.Crdt.Interfaces;
using CloudState.CSharpSupport.Interfaces.Crdt.Contexts;
using Optional;

namespace CloudState.CSharpSupport.Crdt
{
    internal class EntityRunner
    {
        private bool CrdtIsNew { get; set; }
        private Dictionary<long, Func<ISubscriptionContext, Option<object>>> Subscribers = new
            Dictionary<long, Func<ISubscriptionContext, Option<object>>>();
        private Dictionary<long, Action<IStreamCancelledContext>> CancelListeners = new 
            Dictionary<long, Action<IStreamCancelledContext>>();

        private string EntityId { get; }
        private ICrdtStatefulService Service { get; }
        private object Entity { get; }
        private Option<IInternalCrdt> Crdt { get; }

        public EntityRunner(ICrdtStatefulService service, String entityId, Option<IInternalCrdt> crdt)
        {
            Crdt = crdt;
            EntityId = entityId;
            Service = service;
            var ctx = new CrdtEntityCreationContext(); //with CapturingCrdtFactory with ActivatableContext
            try
            {
                Entity = service.Factory.CreateEntityHandler(ctx);
            } 
            finally
            {
                ctx.Deactivate();
            }
            // verifyNoDelta("creation")
        }

        public List<CrdtStreamedMessage> HandleDelta(CrdtDelta delta)
        {
            Crdt.Match(
                x => x.ApplyDelta(delta), // TODO: Apply or else?
                () => throw new InvalidOperationException("Received delta for CRDT before it was created."));
            NotifySubscribers();
            return null;
        }

        private void NotifySubscribers()
        {
            throw new NotImplementedException();
        }

        public List<CrdtStreamOut> HandleCommand(Command command)
        {
            var isStreamed = Service.IsStreamed(command.Name);
            var ctx = isStreamed ? new CrdtStreamedCommandContext(command) :
                new CrdtCommandContext(command);
            return null;
        }

//
//        def handleCommand(command: Command): List[CrdtStreamOut] = {
//          val grpcMethodIsStreamed = service.isStreamed(command.name)
//          val ctx = if (grpcMethodIsStreamed) {
//            new CrdtStreamedCommandContext(command)
//          } else {
//            new CrdtCommandContext(command)
//          }
//
//          val reply = try {
//            val payload = ScalaPbAny.toJavaProto(command.payload.get)
//            ctx match {
//              case streamed: CrdtStreamedCommandContext =>
//                entity.handleStreamedCommand(payload, streamed)
//              case regular =>
//                entity.handleCommand(payload, regular)
//            }
//          } catch {
//            case FailInvoked =>
//              Optional.empty[JavaPbAny]()
//          } finally {
//            ctx.deactivate()
//          }
//
//          val clientAction = ctx.createClientAction(reply, allowNoReply = true)
//
//          if (ctx.hasError) {
//            verifyNoDelta("failed command handling")
//            CrdtStreamOut(
//              CrdtStreamOut.Message.Reply(
//                CrdtReply(
//                  commandId = command.id,
//                  clientAction = clientAction
//                )
//              )
//            ) :: Nil
//          } else {
//            val crdtAction = ctx.createCrdtAction()
//
//            // Notify subscribers of any changes before adding this streams subscribers to the list
//            val streamedMessages = if (crdtAction.isDefined) {
//              notifySubscribers()
//            } else Nil
//
//            val streamAccepted = ctx match {
//              case stream: CrdtStreamedCommandContext => stream.addCallbacks()
//              case _ => false
//            }
//
//            CrdtStreamOut(
//              CrdtStreamOut.Message.Reply(
//                CrdtReply(
//                  commandId = command.id,
//                  clientAction = clientAction,
//                  stateAction = crdtAction,
//                  sideEffects = ctx.sideEffects,
//                  streamed = streamAccepted
//                )
//              )
//            ) :: streamedMessages.map(m => CrdtStreamOut(CrdtStreamOut.Message.StreamedMessage(m)))
//          }
//        }


    }
}