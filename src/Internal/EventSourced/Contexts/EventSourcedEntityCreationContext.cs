using CloudState.CSharpSupport.Contexts.Abstractions;
using CloudState.CSharpSupport.Interfaces.EventSourced.Contexts;

namespace CloudState.CSharpSupport.EventSourced.Contexts
{
    internal class EventSourcedEntityCreationContext : DelegatingEventSourcedContext, IEventSourcedEntityCreationContext
    {
        public EventSourcedEntityCreationContext(IEventSourcedContext @delegate)
            : base(@delegate)
        {
        }
    }
}