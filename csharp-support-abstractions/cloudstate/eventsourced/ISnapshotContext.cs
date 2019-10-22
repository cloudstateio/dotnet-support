namespace io.cloudstate.csharpsupport.eventsourced
{
    public interface ISnapshotContext : IEventSourcedContext
    {

        long SequenceNumber { get; }

    }
}