namespace CloudState.CSharpSupport.Interfaces.EventSourced.Contexts
{
    public interface IEventContext : IEventSourcedContext
    {
        long SequenceNumber { get; }
    }

    public interface IEventBehaviorContext : IEventContext, IBehaviorContext
    {

    }
}
