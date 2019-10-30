using System.Diagnostics.CodeAnalysis;
using CloudState.CSharpSupport.Attributes.EventSourced;
using CloudState.CSharpSupport.Tests.AttributeBasedEntityFactory.CommandHandler.Messages;
using Xunit;

namespace CloudState.CSharpSupport.Tests.AttributeBasedEntityFactory.CommandHandler.Entities
{
    [EventSourcedEntity]
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
    public class SingleArgCommandHandlerEntity
    {
        [CommandHandler]
        [SuppressMessage("ReSharper", "UnusedMember.Global")]
        public Wrapped AddItem(string msg)
        {
            return new Wrapped(msg);
        }
    }
}