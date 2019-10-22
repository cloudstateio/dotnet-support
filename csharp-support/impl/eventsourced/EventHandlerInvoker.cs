using System;
using System.Linq;
using System.Reflection;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using io.cloudstate.csharpsupport.eventsourced;
using static io.cloudstate.csharpsupport.impl.ReflectionHelper;

namespace io.cloudstate.csharpsupport.impl.eventsourced
{
    public class EventHandlerInvoker
    {

        MethodInfo Method { get; }

        public EventHandlerAttribute Attribute { get; }
        internal ParameterHandler[] Parameters { get; }

        public EventHandlerInvoker(MethodInfo method)
        {

            Method = method;

            var attr = method.GetCustomAttribute(typeof(EventHandlerAttribute)) as EventHandlerAttribute;
            if (attr == null)
                throw new ArgumentNullException(
                    $"Target event handler method [{method.Name}] is not decorated with [{nameof(EventHandlerAttribute)}]"
                );
            Attribute = attr;
            Parameters = ReflectionHelper.GetParameterHandlers<IEventBehaviorContext>(method);

            //   private def annotationEventClass = annotation.eventClass() match {
            //     case obj if obj == classOf[Object] => None
            //     case clazz => Some(clazz)
            //   }

        }

        public void Invoke(Object obj, Object @event, IEventBehaviorContext context)
        {
            try
            {
                var ctx = new InvocationContext(@event, context);
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
