using System.Collections.Generic;
using CloudState.CSharpSupport.EventSourced.Interfaces;
using CloudState.CSharpSupport.Interfaces.EventSourced;
using CloudState.CSharpSupport.Interfaces.EventSourced.Contexts;
using CloudState.CSharpSupport.Reflection.Interfaces;
using CloudState.CSharpSupport.Serialization;
using Google.Protobuf.Reflection;

namespace CloudState.CSharpSupport.EventSourced.Abstract
{
    internal abstract class StatefulEntityService : IStatefulEntityService
    {
        private IEntityHandlerFactory EntityHandlerFactory { get; }

        public IReadOnlyDictionary<string, IResolvedServiceMethod> Methods { get; }
        public ServiceDescriptor ServiceDescriptor { get; }
        public string PersistenceId { get; }
        public AnySupport AnySupport { get; }
        public int SnapshotEvery { get; }
        
        public abstract string StatefulServiceTypeName { get; }

        public StatefulEntityService(IEntityHandlerFactory factory, ServiceDescriptor serviceDescriptor, AnySupport anySupport, string persistenceId, int snapshotEvery)
        {
            EntityHandlerFactory = factory;
            ServiceDescriptor = serviceDescriptor;
            AnySupport = anySupport;
            PersistenceId = persistenceId;
            SnapshotEvery = snapshotEvery;
        }

        public IEntityHandler CreateEntityHandler(IEventSourcedContext ctx)
        {
            return EntityHandlerFactory.CreateEntityHandler(ctx);
        }
    }
}
