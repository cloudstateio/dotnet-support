namespace io.cloudstate.csharpsupport.eventsourced
{
    public interface IBehaviorContext : IEventSourcedContext
    {

        void Become(params object[] behaviors);

    }
}