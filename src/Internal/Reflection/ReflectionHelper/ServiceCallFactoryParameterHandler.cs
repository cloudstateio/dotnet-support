using CloudState.CSharpSupport.Contexts;

namespace CloudState.CSharpSupport.Reflection.ReflectionHelper
{
    internal partial class ReflectionHelper
    {
        public class ServiceCallFactoryParameterHandler : ParameterHandler
        {
            public override object Apply(InvocationContext ctx)
            {
                return ((Context)ctx.Context).ServiceCallFactory;
            }
        }

    }
}