using CloudState.CSharpSupport.Interfaces.Contexts;

namespace CloudState.CSharpSupport.Interfaces.EventSourced.Contexts
{
    public interface IEventSourcedEntityCreationContext : IEntityCreationContext, IEventSourcedContext, IBehaviorContext
    {

    }
}