using System;

namespace CloudState.CSharpSupport.Reflection.ReflectionHelper
{
    internal partial class ReflectionHelper
    {
        public class ParameterHandler : InvocationContext
        {
            public virtual object Apply(InvocationContext ctx)
            {
                throw new NotImplementedException();
            }
        }

    }
}