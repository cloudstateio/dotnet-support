using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using CloudState.CSharpSupport.EventSourced.Reflection;
using CloudState.CSharpSupport.Reflection.Interfaces;
using Optional;
using static CloudState.CSharpSupport.Reflection.ReflectionHelper.ReflectionHelper;
using Type = System.Type;

namespace CloudState.CSharpSupport.Reflection
{
    internal class ReflectionCacheBehaviorResolver : IBehaviorResolver
    {
        private Func<IReadOnlyDictionary<string, IResolvedServiceMethod>> ResolvedMethodAccessor { get; }

        private ConcurrentDictionary<Type, EventBehaviorReflection> BehaviorReflectionCache { get; }
            = new ConcurrentDictionary<Type, EventBehaviorReflection>();

        public ReflectionCacheBehaviorResolver(Func<IReadOnlyDictionary<string, IResolvedServiceMethod>> resolvedMethodAccessor)
        {
            ResolvedMethodAccessor = resolvedMethodAccessor;
        }

        public EventBehaviorReflection GetOrAdd(Type type)
        {
            return BehaviorReflectionCache.GetOrAdd(
                type,
                EventBehaviorReflection.Create(
                    type,
                    ResolvedMethodAccessor()
                )
            );
        }
    }
}
