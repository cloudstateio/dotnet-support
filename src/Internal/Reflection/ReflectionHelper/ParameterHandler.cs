using System;
using CloudState.CSharpSupport.Interfaces.Contexts;

namespace CloudState.CSharpSupport.Reflection.ReflectionHelper
{
    internal partial class ReflectionHelper
    {
        public class ParameterHandler<TContext> : InvocationContext<TContext>
            where TContext : IContext
        {
            public virtual object Apply(InvocationContext<TContext> ctx)
            {
                throw new NotImplementedException();
            }
        }

    }
}