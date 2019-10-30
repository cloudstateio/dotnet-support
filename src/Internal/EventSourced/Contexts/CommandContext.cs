using System;
using System.Collections.Generic;
using CloudState.CSharpSupport.Contexts.Interfaces;
using CloudState.CSharpSupport.Interfaces.EventSourced;
using CloudState.CSharpSupport.Interfaces.EventSourced.Contexts;
using CloudState.CSharpSupport.Interfaces.Services;
using CloudState.CSharpSupport.Serialization;
using Google.Protobuf.WellKnownTypes;

namespace CloudState.CSharpSupport.EventSourced.Contexts
{
    internal class CommandContext : ICommandContext
    //   with AbstractClientActionContext
    {
        private List<Any> Events { get; } = new List<Any>();
        private bool PerformSnapshot { get; set; } = false;


        private AnySupport AnySupport { get; }
        private IEntityHandler EntityHandler { get; }
        private int SnapshotEvery { get; }

        // Composite contexts
        private AbstractContext AbstractContext { get; }
        private IEffectContext AbstractEffectContext { get; }

        private IActivatableContext ActivatableContext { get; }
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
            IEffectContext abstractEffectContext,
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
            AbstractEffectContext = abstractEffectContext;
            ActivatableContext = activatableContext;
        }

        public void Emit(object @event)
        {
            throw new NotImplementedException();
        }
        // ICommandContext.IEffectContext
        public void Effect(IServiceCall effect, bool synchronous) => AbstractEffectContext.Effect(effect, synchronous);
    }
}
