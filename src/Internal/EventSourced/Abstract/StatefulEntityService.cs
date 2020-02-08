using System.Collections.Generic;
using CloudState.CSharpSupport.Interfaces;
using CloudState.CSharpSupport.Interfaces.Contexts;
using CloudState.CSharpSupport.Reflection.Interfaces;
using CloudState.CSharpSupport.Serialization;
using Google.Protobuf.Reflection;

namespace CloudState.CSharpSupport.EventSourced.Abstract
{
    internal abstract class StatefulEntityService<TContext, THandler> : IStatefulEntityService<TContext, THandler>
        where TContext : IEntityContext
        where THandler : IEntityHandler
    {
        private IEntityHandlerFactory<TContext, THandler> EntityHandlerFactory { get; }
        public ServiceDescriptor ServiceDescriptor { get; }
        public AnySupport AnySupport { get; }
        public IReadOnlyDictionary<string, IResolvedServiceMethod> Methods { get; }
        public string PersistenceId { get; }

        public abstract string StatefulServiceTypeName { get; }

        protected StatefulEntityService(IEntityHandlerFactory<TContext, THandler> factory, ServiceDescriptor serviceDescriptor, AnySupport anySupport, string persistenceId = null)
        {
            EntityHandlerFactory = factory;
            ServiceDescriptor = serviceDescriptor;
            AnySupport = anySupport;
            Methods = factory switch
            {
                IResolvedEntityFactory resolved => resolved.ResolvedMethods,
                _ => new Dictionary<string, IResolvedServiceMethod>()
            };
            PersistenceId = persistenceId ?? ServiceDescriptor.Name;
        }

        public THandler CreateEntityHandler(TContext ctx)
        {
            return EntityHandlerFactory.CreateEntityHandler(ctx);
        }
    }
}
