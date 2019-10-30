using System;

namespace CloudState.CSharpSupport.Reflection.ReflectionHelper
{
    internal partial class ReflectionHelper
    {
        public class MainArgumentParameterHandler : ParameterHandler
        {
            public Type Type { get; }
            public MainArgumentParameterHandler(Type type)
            {
                Type = type;
            }
            public override object Apply(InvocationContext ctx)
            {
                return ctx.MainArgument;
            }
        }

    }
}