using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CloudState.CSharpSupport.Attributes.EventSourced;
using CloudState.CSharpSupport.EventSourced.Reflection;
using CloudState.CSharpSupport.Exceptions;
using CloudState.CSharpSupport.Reflection.Interfaces;
using Optional;
using static CloudState.CSharpSupport.Reflection.ReflectionHelper.ReflectionHelper;
using Type = System.Type;

namespace CloudState.CSharpSupport.Reflection
{
    internal class EventBehaviorReflection
    {
        private ConcurrentDictionary<Type, Option<EventHandlerInvoker>> EventHandlerCache { get; }
            = new ConcurrentDictionary<Type, Option<EventHandlerInvoker>>();
        private ConcurrentDictionary<Type, Option<SnapshotHandlerInvoker>> SnapshotHandlerCache { get; }
            = new ConcurrentDictionary<Type, Option<SnapshotHandlerInvoker>>();

        internal IReadOnlyDictionary<string, CommandHandlerInvoker> CommandHandlers { get; }
        private Dictionary<Type, EventHandlerInvoker> EventHandlers { get; }

        private Dictionary<Type, SnapshotHandlerInvoker> SnapshotHandlers { get; }


        private EventBehaviorReflection(IReadOnlyDictionary<string, CommandHandlerInvoker> commandHandlers,
            Dictionary<Type, EventHandlerInvoker> eventHandlers,
            Dictionary<Type, SnapshotHandlerInvoker> snapshotHandlers)
        {
            CommandHandlers = commandHandlers;
            EventHandlers = eventHandlers;
            SnapshotHandlers = snapshotHandlers;
        }

        internal static EventBehaviorReflection Create(Type entityType, IReadOnlyDictionary<string, IResolvedServiceMethod> serviceMethods)
        {
            var allMethods = GetAllDeclaredMethods(entityType);

            var eventHandlers = allMethods
                .Where(type => type.GetCustomAttribute(typeof(EventHandlerAttribute)) != null)
                .Select(method => new EventHandlerInvoker(method))
                .GroupBy(x => x.AttributeEventClass)
                .Select(x =>
                {
                    if (x.Count() > 1)
                    {
                        throw new CloudStateException("Multiple event handlers for the same type not allowed.");
                    }
                    return new { x.Key, e = x.First() };
                })
                .ToDictionary(x => x.Key, x => x.e);

            var commandHandlers = allMethods
                .Where(x => x.GetCustomAttribute(typeof(CommandHandlerAttribute)) != null)
                .Select(method =>
                {
                    var annotation = method.GetCustomAttribute(typeof(CommandHandlerAttribute)) as CommandHandlerAttribute;
                    var name = string.IsNullOrEmpty(annotation?.Name) ? GetCapitalizedName(method) : annotation.Name;

                    if (!serviceMethods.TryGetValue(name, out var serviceMethod))
                    {
                        throw new CloudStateException(
                            $"Command handler method [{method.Name}] for command [{name}] " +
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

            var snapshotHandlers = allMethods
                .Where(x => x.GetCustomAttribute(typeof(SnapshotHandlerAttribute)) != null)
                .Select(method => new SnapshotHandlerInvoker(method))
                .GroupBy(x => x.SnapshotClass)
                .Select(x => x switch
                {
                    { } single when single.Count() == 1 => (single.Key, Value: single.First()),
                    { } many => throw new Exception(
                        $"Multiple methods found for handling snapshot of type [{many.Key}]: " +
                        $"[{string.Join(", ", many.Select(x => x.Method.Name))}]"),
                    _ => throw new InvalidOperationException()
                })
                .ToDictionary(x => x.Key, x => x.Value);

            return new EventBehaviorReflection(commandHandlers, eventHandlers, snapshotHandlers);
        }

        internal Option<EventHandlerInvoker> GetEventHandler(Type eventType)
        {
            return EventHandlerCache.GetOrAdd(
                eventType,
                type => GetHandlerForClass(EventHandlers, type)
            );

        }

        public Option<SnapshotHandlerInvoker> GetSnapshotHandler(Type snapshotType)
        {
            return SnapshotHandlerCache.GetOrAdd(
                snapshotType,
                type => GetHandlerForClass(SnapshotHandlers, type));
        }

        private Option<T> GetHandlerForClass<T>(IReadOnlyDictionary<Type, T> handlers, Type type)
        {
            if (handlers.TryGetValue(type, out var handler))
                return handler.Some();
            foreach (var @interface in type.GetInterfaces())
            {
                var match = GetHandlerForClass(handlers, @interface).Match(
                    x => x.Some(),
                    Option.None<T>
                );
                if (match.HasValue)
                {
                    return match;
                }
            }
            if (type.BaseType != null)
            {
                return GetHandlerForClass(handlers, type.BaseType);
            }
            return Option.None<T>();
        }

    }
}
