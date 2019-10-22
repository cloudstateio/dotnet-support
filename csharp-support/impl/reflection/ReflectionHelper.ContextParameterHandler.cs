namespace io.cloudstate.csharpsupport.impl
{
    internal partial class ReflectionHelper
    {
        public class ContextParameterHandler : ParameterHandler
        {
            public override object Apply(InvocationContext ctx)
            {
                return ctx.Context;
            }
        }

    }
}