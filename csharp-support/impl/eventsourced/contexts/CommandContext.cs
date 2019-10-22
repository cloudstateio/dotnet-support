using System;
using System.Linq;
using System.Collections.Generic;
using Cloudstate;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using io.cloudstate.csharpsupport.eventsourced;
using Optional;
using Option = Optional.Option;
using io.cloudstate.csharpsupport.impl.eventsourced;

namespace io.cloudstate.csharpsupport.impl
{

    /// <summary>
    /// Context which isolates the execution of a single Command against a 
    /// target EventSourcedEntity.
    /// </summary>
    public class CommandContext : ICommandContext, IAbstractClientActionContext,
        IActivateableContext, IAbstractEffectContext
    {
        private IEventSourcedEntityHandler Handler { get; }
        private List<Any> EventList { get; } = new List<Any>();

        public AnySupport AnySupport { get; }
        public string CommandName { get; }
        public long CommandId { get; }
        public string EntityId { get; }
        public bool PerformSnapshot { get; set; }
        public long SequenceNumber { get; }
        public int SnapshotEvery { get; }

        public IServiceCallFactory ServiceCallFactory { get; }
        public IReadOnlyList<Any> Events => EventList;

        bool IActivateableContext.Inactive { get; set; }
        Option<string> IAbstractClientActionContext.Error { get; set; }
        Option<Forward> IAbstractClientActionContext.ForwardMessage { get; set; }

        public CommandContext(
            IServiceCallFactory serviceCallFactory,
            string entityId,
            long sequence,
            string name,
            long id,
            AnySupport anySupport,
            IEventSourcedEntityHandler handler,
            int snapshotEvery)
        {
            ServiceCallFactory = serviceCallFactory;
            EntityId = entityId;
            SequenceNumber = sequence;
            CommandName = name;
            CommandId = id;
            AnySupport = anySupport;
            Handler = handler;
            SnapshotEvery = snapshotEvery;
        }

        #region ICommandContext implementation 

        /// <summary>
        /// Emit the given event. The event will be persisted, and the handler of the event defined in the
        /// current behavior will immediately be executed to pick it up.
        /// </summary>
        public void Emit(Object @event)
        {
            ((IActivateableContext)this).CheckActive();
            var anyEvent = Any.Pack(@event as IMessage);
            var nextSequenceNumber = SequenceNumber + Events.Count + 1;
            Handler.HandleEvent(
                anyEvent,
                new EventContext(EntityId, nextSequenceNumber)
            );
            EventList.Add(anyEvent);
            PerformSnapshot = (SnapshotEvery > 0) && (PerformSnapshot || (nextSequenceNumber % SnapshotEvery == 0));
        }

        #endregion

        public Exception Fail(string errorMessage)
        {
            ((IActivateableContext)this).CheckActive();

            if (!((IAbstractClientActionContext)this).Error.HasValue)
            {
                ((IAbstractClientActionContext)this).Error = Option.Some(errorMessage);
                throw new FailInvokedException();
            }

            throw new InvalidOperationException("fail(...) already previously invoked!");
        }

        public void Forward(IServiceCall to)
        {
            throw new NotImplementedException("Forward");
        }

        public void Effect(IServiceCall effect, bool synchronous)
        {
            ((IAbstractEffectContext)this).Effect(effect, synchronous);
        }
    }
}