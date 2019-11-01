using System.Diagnostics.CodeAnalysis;
using CloudState.CSharpSupport.Attributes.EventSourced;
using CloudState.CSharpSupport.Tests.AttributeBasedEntityFactory.CommandHandler.Messages;

namespace CloudState.CSharpSupport.Tests.AttributeBasedEntityFactory.EventHandler.Entities
{
    [EventSourcedEntity]
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
    public class NoArgEventHandlerEntity
    {
        public bool Invoked { get; private set; }

        [SuppressMessage("ReSharper", "UnusedMember.Global")]
        [EventHandler(typeof(string))]
        public void Handle()
        {
            Invoked = true;
        }
    }
}
