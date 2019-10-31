using CloudState.CSharpSupport.Interfaces.EventSourced.Contexts;
using CloudState.CSharpSupport.Interfaces.Services;

namespace CloudState.CSharpSupport.Contexts.Abstractions
{
    internal abstract class DelegatingEventSourcedContext : DelegatingEventSourcedContext<IEventSourcedContext>
    {
        protected DelegatingEventSourcedContext(IEventSourcedContext @delegate) : base(@delegate)
        {
        }
    }
        
    internal abstract class DelegatingEventSourcedContext<T> : IEventSourcedContext
        where T : IEventSourcedContext
    {
        protected T Delegate { get; }

        public string EntityId => Delegate.EntityId;
        public IServiceCallFactory ServiceCallFactory => Delegate.ServiceCallFactory;

        protected DelegatingEventSourcedContext(T @delegate)
        {
            Delegate = @delegate;
        }
    }
}
