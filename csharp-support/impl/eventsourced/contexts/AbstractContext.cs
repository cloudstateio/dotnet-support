using System.Diagnostics.CodeAnalysis;
using io.cloudstate.csharpsupport.eventsourced;

namespace io.cloudstate.csharpsupport.impl
{
    [ExcludeFromCodeCoverage]
    public abstract class AbstractContext : IEventSourcedContext
    {
        public string EntityId { get; }

        public IServiceCallFactory ServiceCallFactory { get; }

        protected AbstractContext(string entityId, IServiceCallFactory serviceCallFactory)
        {
            EntityId = entityId;
            ServiceCallFactory = serviceCallFactory;
        }
    }
}