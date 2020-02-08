using CloudState.CSharpSupport.EventSourced.Contexts.Interfaces;
using CloudState.CSharpSupport.Interfaces.EventSourced.Contexts;

namespace CloudState.CSharpSupport.EventSourced.Contexts
{
    internal class EventContext : EventSourcedContext, IEventContext
    {
        public long SequenceNumber { get; }

        public EventContext(string entityId, long sequenceNumber, IAbstractContext abstractContext)
            : base(entityId, abstractContext)
        {
            SequenceNumber = sequenceNumber;
        }
    }
}