using System;
using CloudState.CSharpSupport.Contexts.Abstractions;
using CloudState.CSharpSupport.Interfaces.EventSourced.Contexts;

namespace CloudState.CSharpSupport.EventSourced.Contexts
{
    internal class EventSourcedEntityCreationContext : DelegatingEventSourcedContext, IEventSourcedEntityCreationContext
    {
        private Action<object[]> Become { get; }
        
        public EventSourcedEntityCreationContext(IEventSourcedContext @delegate, Action<object[]> become)
            : base(@delegate)
        {
            Become = become;
        }

        void IBehaviorContext.Become(params object[] behaviors) => Become(behaviors);
    }
}