namespace io.cloudstate.csharpsupport.eventsourced
{
    public interface IEventContext : IEventSourcedContext
    {
        long SequenceNumber { get; }
    }
}