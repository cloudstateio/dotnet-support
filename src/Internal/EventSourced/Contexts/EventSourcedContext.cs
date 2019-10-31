using System;
using CloudState.CSharpSupport.Contexts.Abstractions;
using CloudState.CSharpSupport.EventSourced.Contexts.Interfaces;
using CloudState.CSharpSupport.Interfaces.EventSourced.Contexts;
using CloudState.CSharpSupport.Interfaces.Services;

namespace CloudState.CSharpSupport.EventSourced.Contexts
{
    internal class EventBehaviorContext : DelegatingEventSourcedContext, IEventBehaviorContext
    {
        private Action<object[]> Become { get; }
        public long SequenceNumber { get; }

        public EventBehaviorContext(IEventSourcedContext @delegate, Action<object[]> become) : base(@delegate)
        {
            Become = become;
        }

        void IBehaviorContext.Become(params object[] behaviors) => Become(behaviors);
    }
    
    internal class EventSourcedContext : IEventSourcedContext, IAbstractContext
    {
        private IAbstractContext AbstractContext { get; }

        public string EntityId { get; }
        public IServiceCallFactory ServiceCallFactory => AbstractContext.ServiceCallFactory;

        public EventSourcedContext(string entityId, IAbstractContext abstractContext)
        {
            EntityId = entityId;
            AbstractContext = abstractContext;
        }
    }
}
