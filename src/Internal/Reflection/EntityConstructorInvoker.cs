using System.Linq;
using System.Reflection;
using CloudState.CSharpSupport.Exceptions;
using CloudState.CSharpSupport.Interfaces.EventSourced.Contexts;
using static CloudState.CSharpSupport.Reflection.ReflectionHelper.ReflectionHelper;

namespace CloudState.CSharpSupport.Reflection
{
    public class EntityConstructorInvoker
    {
        private ConstructorInfo Constructor { get; }
        private ParameterHandler[] Parameters { get; }

        public EntityConstructorInvoker(ConstructorInfo constructor)
        {
            Constructor = constructor;
            Parameters = GetParameterHandlers<IEventSourcedEntityCreationContext>(constructor);
            foreach (var parameter in Parameters)
                switch (parameter)
                {
                    case MainArgumentParameterHandler mainArg:
                        throw new InvalidEntityConstructorParameterException(mainArg.Type);
                }
        }

        public object Apply(IEventSourcedEntityCreationContext context)
        {
            var ctx = new InvocationContext("", context);
            return Constructor.Invoke(Parameters.Select(x => x.Apply(ctx)).ToArray());
        }
    }
}
