using CloudState.CSharpSupport.Interfaces.Contexts;

namespace CloudState.CSharpSupport.Interfaces
{
    public interface IEntityHandlerFactory<in TContext, out THandler>
        where TContext : IEntityContext
    {
        THandler CreateEntityHandler(TContext ctx);
    }
}
