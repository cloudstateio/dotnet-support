using System;
using CloudState.CSharpSupport.Interfaces.Contexts;

namespace CloudState.CSharpSupport.Reflection.ReflectionHelper
{
    internal partial class ReflectionHelper
    {
        public class MainArgumentParameterHandler<TContext> : ParameterHandler<TContext>
            where TContext : IContext
        {
            public Type Type { get; }
            public MainArgumentParameterHandler(Type type)
            {
                Type = type;
            }
            public override object Apply(InvocationContext<TContext> ctx)
            {
                return ctx.MainArgument;
            }
        }

    }
}