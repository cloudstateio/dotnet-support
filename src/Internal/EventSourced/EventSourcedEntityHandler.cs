using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using CloudState.CSharpSupport.Contexts.Abstractions;
using CloudState.CSharpSupport.EventSourced.Contexts;
using CloudState.CSharpSupport.Exceptions;
using CloudState.CSharpSupport.Interfaces.EventSourced;
using CloudState.CSharpSupport.Interfaces.EventSourced.Contexts;
using CloudState.CSharpSupport.Reflection;
using CloudState.CSharpSupport.Reflection.Interfaces;
using CloudState.CSharpSupport.Serialization;
using Google.Protobuf.WellKnownTypes;
using Optional;
using Option = Optional.Option;

namespace CloudState.CSharpSupport.EventSourced
{
    internal class EventSourcedEntityHandler : IEntityHandler
    {
        private AnySupport AnySupport { get; }
        private Func<IEventSourcedEntityCreationContext, object> EntityCreationFactory { get; }
        private IBehaviorResolver BehaviorReflectionCache { get; }
        private object[] CurrentBehaviors { get; set; } = { };
        private IEventSourcedContext Context { get; }
        private string BehaviorsString => CurrentBehaviors?.Aggregate("", (agg, cur) => agg + ", " + cur.GetType()) ?? "";

        internal EventSourcedEntityHandler(AnySupport anySupport, IEventSourcedContext context, IBehaviorResolver behaviorReflectionCache, Func<IEventSourcedEntityCreationContext, object> entityCreationFactory = null)
        {
            AnySupport = anySupport;
            Context = context;
            EntityCreationFactory = entityCreationFactory;
            BehaviorReflectionCache = behaviorReflectionCache;

            var explicitBehaviors = Option.None<object[]>();
            var active = true;
            void Become(object[] behaviors)
            {
                // ReSharper disable once AccessToModifiedClosure
                if (!active) throw new InvalidOperationException("Context is not active!");
                explicitBehaviors = Option.Some(ValidateBehaviors(behaviors).ToArray());
            }

            var ctx = new EventSourcedEntityCreationContext(context, Become);
            var entity = EntityCreationFactory(ctx);
            active = false;
            CurrentBehaviors = explicitBehaviors.Match(behaviors => behaviors, () => new [] {entity});
        }
        
        private IEnumerable<object> ValidateBehaviors(IEnumerable<object> behaviors)
        {
            foreach (var behavior in behaviors)
            {
                GetCachedBehaviorReflection(behavior);
                yield return behavior;
            }
        }

        public Option<Any> HandleCommand(Any command, ICommandContext context)
        {
            return Unwrap(() =>
            {
                Func<Any> AlternativeFactory(string commandName, string behaviors) =>
                    () => throw new CloudStateException(
                        $"No command handler found for command [{commandName}] on any of the current behaviors: {behaviors}"
                    );

                return CurrentBehaviors
                    .Take(1)
                    .Select(behavior =>
                        GetCachedBehaviorReflection(behavior).CommandHandlers[context.CommandName]
                            .Invoke(behavior, command, context))
                    .FirstOrDefault()
                    .Or(AlternativeFactory(context.CommandName, BehaviorsString));
            });
        }

        public void HandleEvent(Any @event, IEventContext context)
        {
            Unwrap(() =>
            {
                var obj = AnySupport.Decode(@event);
                if (!CurrentBehaviors
                    .Take(1)
                    .Any(behavior => GetCachedBehaviorReflection(behavior)
                        .GetEventHandler(obj.GetType())
                        .Match(handler => {
                                var active = true;
                                var ctx = new EventBehaviorContext(context, behaviors =>
                                {
                                    // ReSharper disable once AccessToModifiedClosure
                                    if (!active) throw new InvalidOperationException("Context is not active!");
                                    CurrentBehaviors = ValidateBehaviors(behaviors).ToArray();
                                });
                                handler.Invoke(behavior, obj, ctx);
                                active = false;
                                return true; 
                            }, 
                            () => false)
                    )
                ) throw new CloudStateException(
                        $"No event handler [{obj.GetType()}] found for any of the current behaviors: {BehaviorsString}");
            });
        }

        public void HandleSnapshot(Any snapshot, ISnapshotContext context)
        {
            throw new NotImplementedException();
        }

        public Option<Any> Snapshot(ISnapshotContext context)
        {
            throw new NotImplementedException();
        }

        private EventBehaviorReflection GetCachedBehaviorReflection(object behavior)
        {
            return BehaviorReflectionCache.GetOrAdd(behavior.GetType());
        }
        
        /// <summary>
        /// Helper function to unwrap the handler's inner exceptions
        /// </summary>
        /// <param name="func"></param>
        /// <typeparam name="TResult"></typeparam>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private TResult Unwrap<TResult>(Func<TResult> func)
        {
            try
            {
                return func();
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                    throw ex.InnerException;
                throw;
            }
        }
        
        /// <summary>
        /// Helper function to unwrap the handler's inner exceptions 
        /// </summary>
        /// <param name="func"></param>
        /// <exception cref="Exception"></exception>
        private void Unwrap(Action func)
        {
            try
            {
                func();
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                    throw ex.InnerException;
                throw;
            }
        }

    }
}