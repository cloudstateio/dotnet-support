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
        private object[] CurrentBehaviors { get; set; }
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
        
        public Option<Any> HandleCommand(Any command, ICommandContext context)
        {
            return Unwrap(() =>
            {
                // TODO: Need to refine this to be clear on the potential exception resulting from the cascading commandhandler / result lookup 
                var result = CurrentBehaviors
                    .Select(behavior => GetCachedBehaviorReflection(behavior)
                        .CommandHandlers[context.CommandName]
                        .Invoke(behavior, command, context))
                    .FirstOrDefault(x => x.HasValue);
                if (!result.HasValue)
                    throw new CloudStateException(
                        $"No command handler found for command [{context.CommandName}] on any of the " +
                        $"current behaviors: {BehaviorsString}"
                    );
                return result;
            });
        }
        
        public void HandleEvent(Any @event, IEventContext context)
        {
            Unwrap(() =>
            {
                var obj = AnySupport.Decode(@event);
                if (!CurrentBehaviors
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

        public void HandleSnapshot(Any anySnapshot, ISnapshotContext context)
        {
            Unwrap(() =>
            {
                var snapshot = AnySupport.Decode(anySnapshot);
                if (!CurrentBehaviors.Any(behavior => BehaviorReflectionCache.GetOrAdd(behavior.GetType())
                    .GetSnapshotHandler(snapshot.GetType())
                    .Match(handler =>
                    {
                        var active = true;
                        var ctx = new SnapshotBehaviorContext(context, behaviors =>
                        {
                            // TODO: Check sequence number override on this context is set correctly.
                            // ReSharper disable once AccessToModifiedClosure
                            if (!active) throw new InvalidOperationException("Context is not active!");
                            CurrentBehaviors = ValidateBehaviors(behaviors).ToArray();
                        });
                        handler.Invoke(behavior, snapshot, ctx);
                        active = false;
                        return true;
                    }, () => false))
                ) throw new CloudStateException(
                    $"No snapshot handler found for snapshot [{snapshot.GetType()}] on any of the current behaviors [{BehaviorsString}]"
                );
            });
            
        }

        public Option<Any> Snapshot(ISnapshotContext context)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// References the <see cref="BehaviorReflectionCache"/> to get or
        /// set the behavior reflection based on the resolved methods known
        /// to the <see cref="AttributeBasedEntityHandlerFactory"/>.
        /// </summary>
        /// <param name="behavior"></param>
        /// <returns></returns>
        private EventBehaviorReflection GetCachedBehaviorReflection(object behavior)
        {
            return BehaviorReflectionCache.GetOrAdd(behavior.GetType());
        }
        
        /// <summary>
        /// Eagerly reflects upon the behavior for handler validation
        /// and returns a copy of the enumerable.
        /// </summary>
        /// <param name="behaviors"></param>
        /// <returns></returns>
        private IEnumerable<object> ValidateBehaviors(IEnumerable<object> behaviors)
        {
            foreach (var behavior in behaviors)
            {
                GetCachedBehaviorReflection(behavior);
                yield return behavior;
            }
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