using CloudState.CSharpSupport.Interfaces.EventSourced.Contexts;
using CloudState.CSharpSupport.Interfaces.Services;

namespace CloudState.CSharpSupport.EventSourced.Contexts
{
    internal class SnapshotContext : ISnapshotContext
    {
        public string EntityId { get; }
        public long SequenceNumber { get; }
        public IServiceCallFactory ServiceCallFactory { get; }

        public SnapshotContext(string entityId, long sequence, IServiceCallFactory serviceCallFactory)
        {
            EntityId = entityId;
            SequenceNumber = sequence;
            ServiceCallFactory = serviceCallFactory;
        }
    }
}