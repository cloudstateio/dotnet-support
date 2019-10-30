using CloudState.CSharpSupport.Serialization;

namespace CloudState.CSharpSupport.EventSourced.Interfaces
{
    internal interface IEventSourcedStatefulService : IStatefulEntityService
    {
        AnySupport AnySupport { get; }
        int SnapshotEvery { get; }
    }
}
