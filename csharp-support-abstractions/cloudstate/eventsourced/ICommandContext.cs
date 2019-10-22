using System;

namespace io.cloudstate.csharpsupport.eventsourced
{
    public interface ICommandContext : IEventSourcedContext, IClientActionContext, IEffectContext
    {

        long CommandId { get; }

        long SequenceNumber { get; }

        String CommandName { get; }

        void Emit(Object @event);

    }
}