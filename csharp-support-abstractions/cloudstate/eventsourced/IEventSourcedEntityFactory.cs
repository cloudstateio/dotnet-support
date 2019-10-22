namespace io.cloudstate.csharpsupport.eventsourced
{

    public interface IEventSourcedEntityFactory
    {

        IEventSourcedEntityHandler Create(IEventSourcedContext context);

    }

}
