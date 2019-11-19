using CloudState.CSharpSupport.Interfaces.Contexts;

namespace CloudState.CSharpSupport.Reflection.ReflectionHelper
{
    internal partial class ReflectionHelper
    {
        internal class InvocationContext<TContext> 
            where TContext : IContext
        {
            public object MainArgument { get; }
            public TContext Context { get; }

            internal InvocationContext(object mainArgument, TContext context)
            {
                MainArgument = mainArgument;
                Context = context;
            }

            internal InvocationContext()
            {

            }
        }

    }
}