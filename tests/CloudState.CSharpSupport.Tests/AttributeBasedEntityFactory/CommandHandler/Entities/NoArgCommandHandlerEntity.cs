using System.Diagnostics.CodeAnalysis;
using CloudState.CSharpSupport.Attributes.EventSourced;
using CloudState.CSharpSupport.Tests.AttributeBasedEntityFactory.CommandHandler.Messages;

namespace CloudState.CSharpSupport.Tests.AttributeBasedEntityFactory.CommandHandler.Entities
{
    [EventSourcedEntity]
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
    public class NoArgCommandHandlerEntity
    {
        [CommandHandler]
        [SuppressMessage("ReSharper", "UnusedMember.Global")]
        public Wrapped AddItem()
        {
            return new Wrapped("blah");
        }
    }
}
