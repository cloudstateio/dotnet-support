using CloudState.CSharpSupport.Interfaces;
using CloudState.CSharpSupport.Interfaces.Contexts;
using CloudState.CSharpSupport.Interfaces.EventSourced;
using CloudState.CSharpSupport.Interfaces.EventSourced.Contexts;
using CloudState.CSharpSupport.Serialization;

namespace CloudState.CSharpSupport.EventSourced.Interfaces
{
    internal interface IEventSourcedStatefulService : IStatefulEntityService<IEventSourcedContext, IEventSourcedEntityHandler>
    {
        AnySupport AnySupport { get; }
        int SnapshotEvery { get; }
    }
}
