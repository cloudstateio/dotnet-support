using System.Diagnostics.CodeAnalysis;
using CloudState.CSharpSupport.Attributes.EventSourced;
using CloudState.CSharpSupport.Interfaces.EventSourced.Contexts;
using CloudState.CSharpSupport.Tests.AttributeBasedEntityFactory.CommandHandler.Messages;

namespace CloudState.CSharpSupport.Tests.AttributeBasedEntityFactory.CommandHandler.Entities
{
    [EventSourcedEntity]
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
    public class BadContextTypeEntity
    {
        [CommandHandler]
        [SuppressMessage("ReSharper", "UnusedParameter.Global")]
        public Wrapped AddItem(string msg, IEventContext context)
        {
            return new Wrapped(msg);
        }
    }
}