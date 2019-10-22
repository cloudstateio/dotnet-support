using System.Diagnostics.CodeAnalysis;
using io.cloudstate.csharpsupport.eventsourced;

namespace io.cloudstate.csharpsupport.impl
{
    [ExcludeFromCodeCoverage]
    public class EventSourcedContext : AbstractContext
    {
        public EventSourcedContext(string entityId, IServiceCallFactory serviceCallFactory)
            : base(entityId, serviceCallFactory)
        {

        }
    }
}