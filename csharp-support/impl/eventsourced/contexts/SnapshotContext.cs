using io.cloudstate.csharpsupport.eventsourced;

namespace io.cloudstate.csharpsupport.impl
{
    public class SnapshotContext : ISnapshotContext
    {
        public long SequenceNumber { get; }

        public string EntityId { get; }

        public IServiceCallFactory ServiceCallFactory { get; }

        public SnapshotContext(string entityId, long sequenceNumber,
            IServiceCallFactory factory)
        {
            EntityId = entityId;
            SequenceNumber = sequenceNumber;
            ServiceCallFactory = factory;
        }
    }
}