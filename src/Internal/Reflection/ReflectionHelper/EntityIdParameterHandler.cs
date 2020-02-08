using CloudState.CSharpSupport.Interfaces.Contexts;
using CloudState.CSharpSupport.Interfaces.EventSourced.Contexts;

namespace CloudState.CSharpSupport.Reflection.ReflectionHelper
{
    internal partial class ReflectionHelper
    {
        public class EntityIdParameterHandler<TContext> : ParameterHandler<TContext>
            where TContext : IContext
        {
            public override object Apply(InvocationContext<TContext> ctx)
            {
                return ((IEntityContext)ctx.Context).EntityId;
            }
        }

    }
}