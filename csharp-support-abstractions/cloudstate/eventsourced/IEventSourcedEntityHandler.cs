using System;
using Google.Protobuf.WellKnownTypes;
using Optional;

namespace io.cloudstate.csharpsupport.eventsourced
{
    public interface IEventSourcedEntityHandler
    {
        void HandleEvent(Any @event, IEventContext context);

        Option<Any> HandleCommand(Any command, ICommandContext context);

        void HandleSnapshot(Any snapshot, ISnapshotContext context);

        Option<Any> Snapshot(ISnapshotContext context);

    }

}