namespace io.cloudstate.csharpsupport.impl
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