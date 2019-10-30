using CloudState.CSharpSupport.Interfaces.Contexts;

namespace CloudState.CSharpSupport.Interfaces.EventSourced.Contexts
{
    public interface IEntityContext : IContext
    {
        string EntityId { get; }
    }
}
