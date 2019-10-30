using CloudState.CSharpSupport.Interfaces;
using CloudState.CSharpSupport.Interfaces.EventSourced;
using CloudState.CSharpSupport.Interfaces.EventSourced.Contexts;

namespace CloudState.CSharpSupport.EventSourced.Interfaces
{
    internal interface IStatefulEntityService : IStatefulService
    {
        IEntityHandler CreateEntityHandler(IEventSourcedContext ctx);
    }
}