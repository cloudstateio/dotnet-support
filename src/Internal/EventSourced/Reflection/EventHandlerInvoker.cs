using System;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.ComTypes;
using CloudState.CSharpSupport.Attributes.EventSourced;
using CloudState.CSharpSupport.Interfaces.EventSourced.Contexts;
using CloudState.CSharpSupport.Reflection.ReflectionHelper;
using Optional;

namespace CloudState.CSharpSupport.EventSourced.Reflection
{
    public class EventHandlerInvoker
    {
        private MethodInfo Method { get; }
        private EventHandlerAttribute Attribute { get; }
        private ReflectionHelper.ParameterHandler[] Parameters { get; }
        internal Type AttributeEventClass { get; }

        public EventHandlerInvoker(MethodInfo method)
        {
            Method = method;
            Attribute = method.GetCustomAttribute<EventHandlerAttribute>() ?? throw new ArgumentNullException(
                            $"Target event handler method [{method.Name}] is not decorated with [{nameof(EventHandlerAttribute)}]"
                        );
            Parameters = ReflectionHelper.GetParameterHandlers<IEventBehaviorContext>(method);
            AttributeEventClass = Attribute.EventClass ?? typeof(Object);
        }

        public void Invoke(object obj, object @event, IEventBehaviorContext context)
        {
            try
            {
                var ctx = new ReflectionHelper.InvocationContext(@event, context);
                Method.Invoke(obj, Parameters.Select(x => x.Apply(ctx)).ToArray());
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception on the invocation");
                Console.WriteLine(ex);
                throw;
            }
        }
    }
}