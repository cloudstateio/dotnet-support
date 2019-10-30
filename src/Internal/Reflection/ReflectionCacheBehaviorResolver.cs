using System.Collections.Concurrent;
using System.Collections.Generic;
using CloudState.CSharpSupport.Reflection.Interfaces;
using static CloudState.CSharpSupport.Reflection.ReflectionHelper.ReflectionHelper;
using Type = System.Type;

namespace CloudState.CSharpSupport.Reflection
{
    internal class ReflectionCacheBehaviorResolver : IBehaviorResolver
    {
        private ConcurrentDictionary<Type, EventBehaviorReflection> BehaviorReflectionCache { get; }
            = new ConcurrentDictionary<Type, EventBehaviorReflection>();

        public ReflectionCacheBehaviorResolver()
        {

        }

        public EventBehaviorReflection GetOrAdd(Type type, IReadOnlyDictionary<string, IResolvedServiceMethod> methods)
        {
            return BehaviorReflectionCache.GetOrAdd(
                type,
                EventBehaviorReflection.Create(
                    type,
                    methods
                )
            );
        }

        public CommandHandlerInvoker GetCommandHandler(object behavior, string commandName)
        {
            return BehaviorReflectionCache[behavior.GetType()].CommandHandlers[commandName];
        }
    }
}
