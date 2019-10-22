using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Google.Protobuf.Reflection;
using io.cloudstate.csharpsupport.eventsourced;
using Optional;

namespace io.cloudstate.csharpsupport.impl.eventsourced
{
    public partial class AnnotationBasedEventSourcedSupport<TClass> :
        IEventSourcedEntityFactory, IResolvedEntityFactory
    {
        protected ConcurrentDictionary<Type, EventBehaviorReflection> BehaviorReflectionCache { get; }
            = new ConcurrentDictionary<Type, EventBehaviorReflection>();

        public AnySupport AnySupport { get; }
        public Func<IEventSourcedContext, TClass> Factory { get; }
        public Dictionary<string, IResolvedServiceMethod> ResolvedMethods { get; }

        public AnnotationBasedEventSourcedSupport(
            AnySupport anySupport,
            Dictionary<string, IResolvedServiceMethod> resolvedMethods,
            Func<IEventSourcedContext, TClass>? factory = null)
        {
            AnySupport = anySupport;
            ResolvedMethods = resolvedMethods;
            Factory = Constructor(factory);
            CreateCache();
        }

        public AnnotationBasedEventSourcedSupport(
            AnySupport anySupport,
            ServiceDescriptor descriptor,
            Func<IEventSourcedContext, TClass>? factory = null)
        {
            AnySupport = anySupport;
            ResolvedMethods = AnySupport.ResolveServiceDescriptor(descriptor);
            Factory = Constructor(factory);
            CreateCache();
        }

        public IEventSourcedEntityHandler Create(IEventSourcedContext context)
            => new EntityHandler(
                this,
                context
            );

        private IEnumerable<object> ValidateBehaviors(IEnumerable<object> behaviors)
        {
            foreach (var behavior in behaviors)
            {
                GetCachedBehaviorReflection(behavior);
            }
            return behaviors;
        }

        private EventBehaviorReflection GetCachedBehaviorReflection(object behavior)
        {
            return BehaviorReflectionCache.GetOrAdd(
                behavior.GetType(),
                x => EventBehaviorReflection.Apply(behavior.GetType(), ResolvedMethods)
            );
        }

        private Func<IEventSourcedContext, TClass> Constructor(Func<IEventSourcedContext, TClass>? factory = null)
        {
            var entityClass = typeof(TClass);
            var entityConstructors = entityClass.GetConstructors();
            if (entityConstructors.Length > 1)
                throw new MultipleEntityConstructorsFoundException(entityClass);
            var entityConstructor = entityConstructors.First();
            return factory ?? new Func<IEventSourcedContext, TClass>(
                context => (TClass)new EntityConstructorInvoker(entityConstructor).Apply(context)
            );
        }

        private void CreateCache()
        {
            var entityClass = typeof(TClass);
            if (!BehaviorReflectionCache.TryAdd(
                entityClass,
                EventBehaviorReflection.Apply(entityClass, ResolvedMethods)
            )) throw new InvalidOperationException("Failed to populate initial BehaviorReflectionCache.");
        }
    }
}
