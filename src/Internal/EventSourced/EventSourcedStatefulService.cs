using CloudState.CSharpSupport.EventSourced.Abstract;
using CloudState.CSharpSupport.Interfaces.EventSourced;
using CloudState.CSharpSupport.Serialization;
using ServiceDescriptor = Google.Protobuf.Reflection.ServiceDescriptor;

namespace CloudState.CSharpSupport.EventSourced
{
    internal class EventSourcedStatefulService : StatefulEntityService
    {
        public override string StatefulServiceTypeName => Cloudstate.Eventsourced.EventSourced.Descriptor.FullName;

        internal EventSourcedStatefulService(IEventSourcedEntityHandlerFactory factory, ServiceDescriptor serviceDescriptor, AnySupport anySupport, string persistenceId, int snapshotEvery)
            : base(factory, serviceDescriptor, anySupport, persistenceId, snapshotEvery)
        {
        }

    }

}