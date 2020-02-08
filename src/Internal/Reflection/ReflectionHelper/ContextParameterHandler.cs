using CloudState.CSharpSupport.Interfaces.Contexts;

namespace CloudState.CSharpSupport.Reflection.ReflectionHelper
{
    internal partial class ReflectionHelper
    {
        public class ContextParameterHandler<TContext> : ParameterHandler<TContext>
            where TContext : IContext
        {
            public override object Apply(InvocationContext<TContext> ctx)
            {
                return ctx.Context;
            }
        }

    }
}