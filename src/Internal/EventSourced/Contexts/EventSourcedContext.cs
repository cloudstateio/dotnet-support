using CloudState.CSharpSupport.EventSourced.Contexts.Interfaces;
using CloudState.CSharpSupport.Interfaces.EventSourced.Contexts;
using CloudState.CSharpSupport.Interfaces.Services;

namespace CloudState.CSharpSupport.EventSourced.Contexts
{
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
