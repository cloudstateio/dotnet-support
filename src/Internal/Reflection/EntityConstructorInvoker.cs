using System.Linq;
using System.Reflection;
using CloudState.CSharpSupport.Exceptions;
using CloudState.CSharpSupport.Interfaces.Contexts;
using CloudState.CSharpSupport.Interfaces.EventSourced.Contexts;
using static CloudState.CSharpSupport.Reflection.ReflectionHelper.ReflectionHelper;

namespace CloudState.CSharpSupport.Reflection
{
    internal class EntityConstructorInvoker<TContext>
        where TContext : IEntityCreationContext
    {
        private ConstructorInfo Constructor { get; }
        private ParameterHandler<TContext>[] Parameters { get; }

        public EntityConstructorInvoker(ConstructorInfo constructor)
        {
            Constructor = constructor;
            Parameters = GetParameterHandlers<TContext>(constructor);
            foreach (var parameter in Parameters)
                switch (parameter)
                {
                    case MainArgumentParameterHandler<TContext> mainArg:
                        throw new InvalidEntityConstructorParameterException(mainArg.Type);
                }
        }

        public object Apply(TContext context)
        {
            var ctx = new InvocationContext<TContext>("", context);
            return Constructor.Invoke(Parameters.Select(x => x.Apply(ctx)).ToArray());
        }
    }
}
