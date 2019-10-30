using CloudState.CSharpSupport.Interfaces.EventSourced.Contexts;
using CloudState.CSharpSupport.Interfaces.Services;

namespace CloudState.CSharpSupport.Contexts.Abstractions
{
    internal abstract class DelegatingEventSourcedContext : IEventSourcedContext
    {
        private IEventSourcedContext Delegate { get; }

        public string EntityId => Delegate.EntityId;
        public IServiceCallFactory ServiceCallFactory => Delegate.ServiceCallFactory;

        protected DelegatingEventSourcedContext(IEventSourcedContext @delegate)
        {
            Delegate = @delegate;
        }
    }
}
