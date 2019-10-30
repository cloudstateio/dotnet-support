using CloudState.CSharpSupport.Interfaces.EventSourced.Contexts;

namespace CloudState.CSharpSupport.Interfaces.EventSourced
{
    public interface IEntityHandlerFactory
    {
        IEntityHandler CreateEntityHandler(IEventSourcedContext ctx);
    }
}
