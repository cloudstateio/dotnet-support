using System;
using System.Collections.Generic;
using CloudState.CSharpSupport.EventSourced.Contexts;
using CloudState.CSharpSupport.Interfaces;
using CloudState.CSharpSupport.Interfaces.EventSourced;
using CloudState.CSharpSupport.Interfaces.EventSourced.Contexts;
using CloudState.CSharpSupport.Reflection;
using CloudState.CSharpSupport.Reflection.Interfaces;
using CloudState.CSharpSupport.Serialization;
using Google.Protobuf.Reflection;

namespace CloudState.CSharpSupport.EventSourced
{
    internal class AttributeBasedEntityHandlerFactory : EntityHandlerFactory<IEventSourcedEntityCreationContext>, IEventSourcedEntityHandlerFactory, IResolvedEntityFactory
    {
        private ReflectionCacheBehaviorResolver BehaviorResolver { get; }

        public AttributeBasedEntityHandlerFactory(
            Type initialClass,
            AnySupport anySupport,
            IReadOnlyDictionary<string, IResolvedServiceMethod> resolvedMethods = null,
            Func<IEventSourcedEntityCreationContext, object> factory = null
        ) :base(initialClass, anySupport, factory, resolvedMethods)
        {
            BehaviorResolver = new ReflectionCacheBehaviorResolver(() => ResolvedMethods);
            InitCache();
        }

        public AttributeBasedEntityHandlerFactory(
            Type initialClass,
            AnySupport anySupport,
            ServiceDescriptor descriptor,
            Func<IEventSourcedContext, object> factory = null
        ) : base(initialClass, anySupport, factory, anySupport.ResolveServiceDescriptor(descriptor))
        {
            BehaviorResolver = new ReflectionCacheBehaviorResolver(() => ResolvedMethods);
            InitCache();
        }
        
        public IEventSourcedEntityHandler CreateEntityHandler(IEventSourcedContext ctx)
        {
            return new EventSourcedEntityHandler(
                AnySupport,
                ctx,
                BehaviorResolver,
                x => Factory.Invoke(x)
            );
        }

//        private object CreateEntity(IEventSourcedEntityCreationContext context)
//        {
//            if (Factory != null)
//            {
//                return Factory.Invoke(context);
//            }
//            return Activator.CreateInstance(InitialClass);
//        }

        private void InitCache()
        {
            BehaviorResolver
                .GetOrAdd(InitialClass);
        }
        
    }
}
