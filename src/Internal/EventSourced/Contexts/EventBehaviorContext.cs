using System;
using CloudState.CSharpSupport.Contexts.Abstractions;
using CloudState.CSharpSupport.Interfaces.EventSourced.Contexts;

namespace CloudState.CSharpSupport.EventSourced.Contexts
{
    internal class EventBehaviorContext : DelegatingEventSourcedContext<IEventContext>, IEventBehaviorContext
    {
        private Action<object[]> Become { get; }
        public long SequenceNumber => Delegate.SequenceNumber;

        public EventBehaviorContext(IEventContext @delegate, Action<object[]> become) : base(@delegate)
        {
            Become = become;
        }

        void IBehaviorContext.Become(params object[] behaviors) => Become(behaviors);
    }
}