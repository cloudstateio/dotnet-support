using System;
using System.Collections.Generic;
using System.Linq;
using CloudState.CSharpSupport.EventSourced.Contexts;
using CloudState.CSharpSupport.Exceptions;
using CloudState.CSharpSupport.Interfaces.EventSourced;
using CloudState.CSharpSupport.Interfaces.EventSourced.Contexts;
using CloudState.CSharpSupport.Reflection;
using Google.Protobuf.WellKnownTypes;
using Optional;

namespace CloudState.CSharpSupport.EventSourced
{
    internal class EventSourcedEntityHandler : IEntityHandler
    {
        private Func<IEventSourcedEntityCreationContext, object> EntityCreationFactory { get; }
        private IBehaviorResolver BehaviorResolver { get; }
        private object[] CurrentBehaviors { get; set; } = { };
        private IEventSourcedContext Context { get; }
        private string BehaviorsString => CurrentBehaviors?.Aggregate("", (agg, cur) => agg + ", " + cur.GetType()) ?? "";

        private IEnumerable<object> GetCurrentBehavior()
        {
            // TODO: Will there be race conditions here?
            if (!CurrentBehaviors.Any())
            {
                CurrentBehaviors = new [] {
                    EntityCreationFactory.Invoke(
                        new EventSourcedEntityCreationContext(Context)
                    )
                };
            }
            return CurrentBehaviors;
        }

        internal EventSourcedEntityHandler(IEventSourcedContext context, IBehaviorResolver behaviorResolver, Func<IEventSourcedEntityCreationContext, object> entityCreationFactory = null)
        {
            Context = context;
            EntityCreationFactory = entityCreationFactory;
            BehaviorResolver = behaviorResolver;
        }

        public Option<Any> HandleCommand(Any command, ICommandContext context)
        {
            // TODO: unwrap
            return GetCurrentBehavior()
                .Take(1)
                .Select(behavior =>
                {
                    var handler = BehaviorResolver.GetCommandHandler(behavior, context.CommandName);
                    return handler.Invoke(behavior, command, context);
                })
                .FirstOrDefault()
                .Or(() => throw new CloudStateException(
                        $"No command handler found for command [{context.CommandName}] on any of the current behaviors: {BehaviorsString}"
                    )
                );
        }

        public void HandleEvent(Any @event, IEventContext context)
        {
            throw new NotImplementedException();
        }

        public void HandleSnapshot(Any snapshot, ISnapshotContext context)
        {
            throw new NotImplementedException();
        }

        public Option<Any> Snapshot(ISnapshotContext context)
        {
            throw new NotImplementedException();
        }

    }
}