using Google.Protobuf.Reflection;

namespace io.cloudstate.csharpsupport.impl.eventsourced
{
    public sealed class EventSourcedStatefulServiceArguments<TEntity>
    {

        public string PersistenceId { get; }
        public int SnapshotEvery { get; }
        public ServiceDescriptor ServiceDescriptor { get; }

        public EventSourcedStatefulServiceArguments(
            string persistenceId,
            int snapshotEvery,
            ServiceDescriptor serviceDescriptor
        )
        {
            PersistenceId = persistenceId;
            SnapshotEvery = snapshotEvery;
            ServiceDescriptor = serviceDescriptor;
        }
    }

    public interface EventSourcedStatefulServiceArguments
    {
        string PersistenceId { get; }
        int SnapshotEvery { get; }
        ServiceDescriptor Descriptor { get; }
    }

}
