using CloudState.CSharpSupport.Interfaces.EventSourced.Contexts;

namespace CloudState.CSharpSupport.Interfaces.EventSourced
{
    public interface IEventSourcedEntityHandlerFactory : IEntityHandlerFactory<IEventSourcedContext, IEventSourcedEntityHandler>
    {

    }
}
