using CloudState.CSharpSupport.Interfaces.Contexts;

namespace CloudState.CSharpSupport.Interfaces.Contexts
{
    public interface IEntityContext : IContext
    {
        string EntityId { get; }
    }
}
