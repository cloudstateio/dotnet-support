using System;
using System.Collections.Generic;
using CloudState.CSharpSupport.Contexts.Abstractions;
using CloudState.CSharpSupport.Contexts.Interfaces;
using CloudState.CSharpSupport.Interfaces.EventSourced;
using CloudState.CSharpSupport.Interfaces.EventSourced.Contexts;
using CloudState.CSharpSupport.Interfaces.Services;
using CloudState.CSharpSupport.Serialization;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;

namespace CloudState.CSharpSupport.EventSourced.Contexts
{
    internal class CommandContext : ICommandContext
    {
        public List<Any> Events { get; } = new List<Any>();
        
        public bool PerformSnapshot { get; private set; } = false;
        
        private AnySupport AnySupport { get; }
        private IEntityHandler EntityHandler { get; }
        private int SnapshotEvery { get; }

        // Composite contexts
        public AbstractContext AbstractContext { get; }
        public AbstractClientActionContext AbstractClientActionContext { get; }
        public AbstractEffectContext AbstractEffectContext { get; }
        public IActivatableContext ActivatableContext { get; }
        // IEntityContext
        public string EntityId { get; }
        // ICommandContext
        public long Sequence { get; }
        public string CommandName { get; }
        public long CommandId { get; }
        // IContext
        public IServiceCallFactory ServiceCallFactory => AbstractContext.ServiceCallFactory;

        public CommandContext(
            string entityId,
            long sequence,
            string commandName,
            long commandId,
            AnySupport anySupport,
            IEntityHandler entityHandler,
            int snapshotEvery,
            AbstractContext abstractContext,
            AbstractClientActionContext abstractClientActionContext,
            AbstractEffectContext abstractEffectContext,
            IActivatableContext activatableContext)
        {
            EntityId = entityId;
            Sequence = sequence;
            CommandName = commandName;
            CommandId = commandId;
            AnySupport = anySupport;
            EntityHandler = entityHandler;
            SnapshotEvery = snapshotEvery;
            AbstractContext = abstractContext;
            AbstractClientActionContext = abstractClientActionContext;
            AbstractEffectContext = abstractEffectContext;
            ActivatableContext = activatableContext;
        }

        public void Emit(object @event)
        {
            ActivatableContext.CheckActive();
            var anyEvent = Any.Pack(@event as IMessage); // TODO: might not be Imessage, may need to pack it.
            var nextSequenceNumber = Sequence + Events.Count + 1;
            EntityHandler.HandleEvent(
                anyEvent,
                new EventContext(EntityId, nextSequenceNumber, AbstractContext)
            );
            Events.Add(anyEvent);
            PerformSnapshot = (SnapshotEvery > 0) && (PerformSnapshot || (nextSequenceNumber % SnapshotEvery == 0));
        }
        
        // ICommandContext.IEffectContext
        public void Effect(IServiceCall effect, bool synchronous) => AbstractEffectContext.Effect(effect, synchronous);
    }
}
