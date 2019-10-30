using CloudState.CSharpSupport.Interfaces.EventSourced.Contexts;

namespace CloudState.CSharpSupport.Reflection.ReflectionHelper
{
    internal partial class ReflectionHelper
    {
        public class EntityIdParameterHandler : ParameterHandler
        {
            public override object Apply(InvocationContext ctx)
            {
                return ((IEntityContext)ctx.Context).EntityId;
            }
        }

    }
}