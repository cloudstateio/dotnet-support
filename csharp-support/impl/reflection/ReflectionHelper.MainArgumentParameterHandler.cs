using System;

namespace io.cloudstate.csharpsupport.impl
{
    internal partial class ReflectionHelper
    {
        public class MainArgumentParameterHandler : ParameterHandler
        {
            public Type Type { get; }
            public object MainArgument { get; }
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