using System.Linq;
using System.Reflection;
using io.cloudstate.csharpsupport.eventsourced;
using static io.cloudstate.csharpsupport.impl.ReflectionHelper;

namespace io.cloudstate.csharpsupport.impl.eventsourced
{
    public class EntityConstructorInvoker
    {
        public ConstructorInfo Constructor { get; }
        internal ParameterHandler[] Parameters { get; }

        public EntityConstructorInvoker(ConstructorInfo constructor)
        {
            Constructor = constructor;
            Parameters = ReflectionHelper.GetParameterHandlers<IEventSourcedContext>(constructor);
            foreach (var parameter in Parameters)
                switch (parameter)
                {
                    case MainArgumentParameterHandler mainArg:
                        throw new InvalidEntityConstructorParameterException(mainArg.Type);
                    default:
                        break;
                }
        }

        public object Apply(IEventSourcedContext context)
        {
            var ctx = new InvocationContext("", context);
            return Constructor.Invoke(Parameters.Select(x => x.Apply(ctx) as object).ToArray());
        }

    }
}
