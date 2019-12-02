using System.Collections.Generic;
using CloudState.CSharpSupport.EventSourced.Abstract;
using CloudState.CSharpSupport.EventSourced.Interfaces;
using CloudState.CSharpSupport.Interfaces.EventSourced;
using CloudState.CSharpSupport.Interfaces.EventSourced.Contexts;
using CloudState.CSharpSupport.Reflection.Interfaces;
using CloudState.CSharpSupport.Serialization;
using ServiceDescriptor = Google.Protobuf.Reflection.ServiceDescriptor;

namespace CloudState.CSharpSupport
{
    internal sealed class EventSourcedStatefulService : StatefulEntityService<IEventSourcedContext, IEventSourcedEntityHandler>, IEventSourcedStatefulService
    {
        public override string StatefulServiceTypeName => Cloudstate.Eventsourced.EventSourced.Descriptor.FullName;
        
        public int SnapshotEvery { get; }

        internal EventSourcedStatefulService(IEventSourcedEntityHandlerFactory factory, ServiceDescriptor serviceDescriptor, AnySupport anySupport, string persistenceId = null, int snapshotEvery = 0)
            : base(factory, serviceDescriptor, anySupport, persistenceId)
        {
            SnapshotEvery = snapshotEvery;
        }
    }

}