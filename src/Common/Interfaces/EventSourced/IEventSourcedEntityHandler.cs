using CloudState.CSharpSupport.Interfaces.EventSourced.Contexts;
using Google.Protobuf.WellKnownTypes;
using Optional;

namespace CloudState.CSharpSupport.Interfaces.EventSourced
{
    public interface IEventSourcedEntityHandler : IEntityHandler
    {
        Option<Any> HandleCommand(Any command, ICommandContext context);
        void HandleEvent(Any @event, IEventContext context);
        void HandleSnapshot(Any snapshot, ISnapshotContext context);
        Option<Any> Snapshot(ISnapshotContext context);
    }
}
