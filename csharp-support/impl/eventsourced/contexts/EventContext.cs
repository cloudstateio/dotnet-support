using System.Diagnostics.CodeAnalysis;
using io.cloudstate.csharpsupport.eventsourced;

namespace io.cloudstate.csharpsupport.impl
{
    [ExcludeFromCodeCoverage]
    public class EventContext : IEventContext
    {
        public long SequenceNumber { get; }

        public string EntityId { get; }

        public IServiceCallFactory ServiceCallFactory { get; }

        public EventContext(
                string entityId,
                long sequenceNumber,
                IServiceCallFactory serviceCallFactory)
        {
            EntityId = entityId;
            SequenceNumber = sequenceNumber;
            ServiceCallFactory = serviceCallFactory;
        }
    }

}