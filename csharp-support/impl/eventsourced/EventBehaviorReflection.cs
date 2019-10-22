using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Google.Protobuf.WellKnownTypes;
using io.cloudstate.csharpsupport.eventsourced;
using Optional;
using static io.cloudstate.csharpsupport.impl.ReflectionHelper;
using Option = Optional.Option;

namespace io.cloudstate.csharpsupport.impl.eventsourced
{
    public class EventBehaviorReflection
    {

        public static EventBehaviorReflection Apply(
            System.Type behaviorClass,
            Dictionary<string, IResolvedServiceMethod> serviceMethods
        )
        {

            var allMethods = ReflectionHelper.GetAllDeclaredMethods(behaviorClass);

            var eventHandlers = allMethods
                .Where(type => type.GetCustomAttribute(typeof(EventHandlerAttribute)) != null)
                .Select(method => new EventHandlerInvoker(method))
                .GroupBy(x => x.Attribute.EventClass)
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
                .Where(type => type.GetCustomAttribute(typeof(CommandHandlerAttribute)) != null)
                .Select(method =>
                {
                    var annotation = method.GetCustomAttribute(typeof(CommandHandlerAttribute)) as CommandHandlerAttribute;
                    var name = String.IsNullOrEmpty(annotation?.Name) ?
                        ReflectionHelper.GetCapitalizedName(method) :
                        annotation.Name;
                    if (!serviceMethods.TryGetValue(name, out var serviceMethod))
                    {
                        throw new CloudStateException(
                            $"Command handler method ${method.Name} for command {name} " +
                            "found, but the service has no command by that name."
                        );
                    }
                    return new ReflectionHelper.CommandHandlerInvoker(
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


            /*
                        
                val snapshotHandlers = allMethods
                .filter(_.getAnnotation(classOf[SnapshotHandler]) != null)
                .map { method =>
                    new SnapshotHandlerInvoker(ReflectionHelper.ensureAccessible(method))
                }
                .groupBy(_.snapshotClass)
                .map {
                    case (snapshotClass, Seq(invoker)) => (snapshotClass: Any) -> invoker
                    case (clazz, many) =>
                    throw new CloudStateException(
                        s"Multiple methods found for handling snapshot of type $clazz: ${many.map(_.method.getName)}"
                    )
                }
                .asInstanceOf[Map[Class[_], SnapshotHandlerInvoker]]

                val snapshotInvoker = allMethods
                .filter(_.getAnnotation(classOf[Snapshot]) != null)
                .map { method =>
                    new SnapshotInvoker(ReflectionHelper.ensureAccessible(method))
                } match {
                case Seq() => None
                case Seq(single) =>
                    Some(single)
                case _ =>
                    throw new CloudStateException(s"Multiple snapshoting methods found on behavior $behaviorClass")
                }

                ReflectionHelper.validateNoBadMethods(
                allMethods,
                classOf[EventSourcedEntity],
                Set(classOf[EventHandler], classOf[CommandHandler], classOf[SnapshotHandler], classOf[Snapshot])
                )

                new EventBehaviorReflection(eventHandlers, commandHandlers, snapshotHandlers, snapshotInvoker)
                */

            Dictionary<System.Type, SnapshotHandlerInvoker> snapshotHandlers = new Dictionary<System.Type, SnapshotHandlerInvoker>();
            return new EventBehaviorReflection(eventHandlers, commandHandlers, snapshotHandlers, Option.None<SnapshotInvoker>());
        }

        ConcurrentDictionary<System.Type, Option<EventHandlerInvoker>> EventHandlerCache { get; }
        ConcurrentDictionary<System.Type, Option<SnapshotHandlerInvoker>> SnapshotHandlerCache { get; }

        internal Dictionary<System.Type, EventHandlerInvoker> EventHandlers { get; }
        internal Dictionary<String, CommandHandlerInvoker> CommandHandlers { get; }
        public Dictionary<System.Type, SnapshotHandlerInvoker> SnapshotHandlers { get; }
        public Option<SnapshotInvoker> SnapshotInvoker { get; }

        internal EventBehaviorReflection(
            Dictionary<System.Type, EventHandlerInvoker> eventHandlers,
            Dictionary<String, ReflectionHelper.CommandHandlerInvoker> commandHandlers,
            Dictionary<System.Type, SnapshotHandlerInvoker> snapshotHandlers,
            Option<SnapshotInvoker> snapshotInvoker)
        {

            EventHandlers = eventHandlers;
            CommandHandlers = commandHandlers;
            SnapshotHandlers = snapshotHandlers;
            SnapshotInvoker = snapshotInvoker;

            EventHandlerCache = new ConcurrentDictionary<System.Type, Option<EventHandlerInvoker>>();
            SnapshotHandlerCache = new ConcurrentDictionary<System.Type, Option<SnapshotHandlerInvoker>>();

        }

        public Option<EventHandlerInvoker> GetCachedEventHandlerForClass(System.Type type)
        {
            return EventHandlerCache.GetOrAdd(
                type,
                type => GetHandlerForClass<EventHandlerInvoker>(EventHandlers, type)
            );

        }

        public Option<SnapshotHandlerInvoker> GetCachedSnapshotHandlerForClass(System.Type type) =>
            SnapshotHandlerCache.GetOrAdd(
                type,
                type => GetHandlerForClass<SnapshotHandlerInvoker>(SnapshotHandlers, type)
            );

        private Option<T> GetHandlerForClass<T>(Dictionary<System.Type, T> handlers, System.Type type)
        {
            if (handlers.TryGetValue(type, out var handler))
                return handler.Some();
            foreach (var @interface in type.GetInterfaces())
            {
                var match = GetHandlerForClass<T>(handlers, @interface).Match(
                    some: x => x.Some(),
                    none: () => Option.None<T>()
                );
                if (match.HasValue)
                {
                    return match;
                }
            }
            if (type.BaseType != null)
            {
                return GetHandlerForClass<T>(handlers, type.BaseType);
            }
            return Option.None<T>();
        }

    }

}
