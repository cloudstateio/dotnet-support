using System;
using System.Collections.Generic;
using System.Linq;
using CloudState.CSharpSupport.Exceptions;
using CloudState.CSharpSupport.Interfaces.EventSourced;
using CloudState.CSharpSupport.Interfaces.EventSourced.Contexts;
using CloudState.CSharpSupport.Reflection;
using CloudState.CSharpSupport.Reflection.Interfaces;
using CloudState.CSharpSupport.Serialization;
using Google.Protobuf.Reflection;

namespace CloudState.CSharpSupport.EventSourced
{
    // Assume the type of entity can and will change over time..
    internal class AttributeBasedEntityHandlerFactory : IEventSourcedEntityHandlerFactory
    {
        private Type InitialClass { get; }
        private IReadOnlyDictionary<string, IResolvedServiceMethod> ResolvedMethods { get; }

        private ReflectionCacheBehaviorResolver BehaviorResolver { get; }
        private AnySupport AnySupport { get; }
        private Func<IEventSourcedEntityCreationContext, object> Factory { get; }

        public AttributeBasedEntityHandlerFactory(
            Type initialClass,
            AnySupport anySupport,
            IReadOnlyDictionary<string, IResolvedServiceMethod> resolvedMethods = null,
            Func<IEventSourcedEntityCreationContext, object> factory = null
        )
        {
            InitialClass = initialClass;
            AnySupport = anySupport;
            ResolvedMethods = resolvedMethods;
            BehaviorResolver = new ReflectionCacheBehaviorResolver(() => ResolvedMethods);
            Factory = Constructor(InitialClass, factory);
            InitCache();
        }

        public AttributeBasedEntityHandlerFactory(
            Type initialClass,
            AnySupport anySupport,
            ServiceDescriptor descriptor,
            Func<IEventSourcedEntityCreationContext, object> factory = null
        )
        {
            InitialClass = initialClass;
            AnySupport = anySupport;
            ResolvedMethods = AnySupport.ResolveServiceDescriptor(descriptor);
            BehaviorResolver = new ReflectionCacheBehaviorResolver(() => ResolvedMethods);
            Factory = Constructor(InitialClass, factory);
            InitCache();
        }

        private Func<IEventSourcedEntityCreationContext, object> Constructor(Type entityClass, Func<IEventSourcedEntityCreationContext, object> factory)
        {
            var entityConstructors = entityClass.GetConstructors();
            if (entityConstructors.Length > 1)
                throw new MultipleEntityConstructorsFoundException(entityClass);
            var entityConstructor = entityConstructors.First();
            return factory ?? (context => new EntityConstructorInvoker(entityConstructor).Apply(context));
        }

        public IEntityHandler CreateEntityHandler(IEventSourcedContext ctx)
        {
            return new EventSourcedEntityHandler(
                AnySupport,
                ctx,
                BehaviorResolver,
                x => Factory.Invoke(x)
            );
        }

        private object CreateEntity(IEventSourcedEntityCreationContext context)
        {
            if (Factory != null)
            {
                return Factory.Invoke(context);
            }
            return Activator.CreateInstance(InitialClass);
        }

        private void InitCache()
        {
            BehaviorResolver
                .GetOrAdd(InitialClass);
        }


    }
}
