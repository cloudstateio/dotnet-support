using System;
using io.cloudstate.csharpsupport.eventsourced;

namespace io.cloudstate.csharpsupport.impl.eventsourced
{
    class DelegatingEventSourcedContext : IEventSourcedContext, IEventBehaviorContext
    {

        public IEventContext Delegate { get; }

        public String EntityId => Delegate.EntityId;
        public IServiceCallFactory ServiceCallFactory => Delegate.ServiceCallFactory;
        public long SequenceNumber => Delegate.SequenceNumber;


        public DelegatingEventSourcedContext(IEventContext @delegate)
        {
            Delegate = @delegate;
        }

        // TODO: active, changing state linked to outside class..
        public void Become(params object[] behaviors)
        {
            throw new NotImplementedException();
        }
    }

}
