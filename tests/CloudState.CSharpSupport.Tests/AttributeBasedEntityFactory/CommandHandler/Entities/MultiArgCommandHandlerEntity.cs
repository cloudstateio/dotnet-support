using System.Diagnostics.CodeAnalysis;
using CloudState.CSharpSupport.Attributes;
using CloudState.CSharpSupport.Attributes.EventSourced;
using CloudState.CSharpSupport.Interfaces.EventSourced.Contexts;
using CloudState.CSharpSupport.Tests.AttributeBasedEntityFactory.CommandHandler.Messages;
using Xunit;

namespace CloudState.CSharpSupport.Tests.AttributeBasedEntityFactory.CommandHandler.Entities
{
    [EventSourcedEntity]
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
    public class MultiArgCommandHandlerEntity
    {
        private IEventSourcedContext Ctx { get; }
        private string Eid { get; }
        
        public MultiArgCommandHandlerEntity(IEventSourcedContext ctx)
        {
            Ctx = ctx;
            Eid = ctx.EntityId;
        }

        [CommandHandler]
        [SuppressMessage("ReSharper", "UnusedMember.Global")]
        [SuppressMessage("ReSharper", "ParameterOnlyUsedForPreconditionCheck.Global")]
        public Wrapped AddItem(string msg, [EntityId]string entityId, ICommandContext commandContext)
        {
            Assert.Equal("blah", msg);
            Assert.Equal(Eid, Ctx.EntityId);
            Assert.Equal(Ctx.EntityId, entityId);
            Assert.Equal("AddItem", commandContext.CommandName);
            return new Wrapped("blah");
        }
    }
}