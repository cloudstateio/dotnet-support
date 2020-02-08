namespace CloudState.CSharpSupport.Interfaces.EventSourced.Contexts
{
    public interface ISnapshotContext : IEventSourcedContext
    {
        long SequenceNumber { get; }
    }
}
