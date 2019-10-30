using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CloudState.CSharpSupport.Attributes.EventSourced;
using CloudState.CSharpSupport.EventSourced;
using CloudState.CSharpSupport.Reflection.Interfaces;
using static CloudState.CSharpSupport.Reflection.ReflectionHelper.ReflectionHelper;
using Type = System.Type;

namespace CloudState.CSharpSupport.Reflection
{
    public class EventBehaviorReflection
    {
        private EventBehaviorReflection(Dictionary<string, CommandHandlerInvoker> commandHandlers)
        {
            CommandHandlers = commandHandlers;
        }

        internal static EventBehaviorReflection Create(Type type, IReadOnlyDictionary<string, IResolvedServiceMethod> serviceMethods)
        {
            var allMethods = GetAllDeclaredMethods(type);

            var commandHandlers = allMethods
                .Where(x => x.GetCustomAttribute(typeof(CommandHandlerAttribute)) != null)
                .Select(method =>
                {

                    var annotation = method.GetCustomAttribute(typeof(CommandHandlerAttribute)) as CommandHandlerAttribute;
                    var name = string.IsNullOrEmpty(annotation?.Name) ?
                        GetCapitalizedName(method) :
                        annotation.Name;

                    if (!serviceMethods.TryGetValue(name, out var serviceMethod))
                    {
                        throw new CloudStateException(
                            $"Command handler method ${method.Name} for command {name} " +
                            "found, but the service has no command by that name."
                        );
                    }

                    return new CommandHandlerInvoker(
                        method,
                        serviceMethod
                    );

                })
                .GroupBy(x => x.ServiceMethod.Name)
                .Select(x =>
                {
                    if (x.Count() > 1)
                    {
                        throw new CloudStateException("Multiple methods for handling the same command type not allowed.");
                    }
                    return new { x.Key, e = x.First() };
                })
                .ToDictionary(x => x.Key, x => x.e);

            return new EventBehaviorReflection(commandHandlers);
        }

        internal IReadOnlyDictionary<string, CommandHandlerInvoker> CommandHandlers { get; }

    }
}
