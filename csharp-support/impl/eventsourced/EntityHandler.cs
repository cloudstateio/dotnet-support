using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Google.Protobuf.WellKnownTypes;
using io.cloudstate.csharpsupport.eventsourced;
using Optional;
using io.cloudstate.csharpsupport.impl;
using Google.Protobuf;

namespace io.cloudstate.csharpsupport.impl.eventsourced
{
    public partial class AnnotationBasedEventSourcedSupport<TClass>
    {

        public class EntityHandler : IEventSourcedEntityHandler
        {
            private AnnotationBasedEventSourcedSupport<TClass> Outer { get; }
            private AnySupport AnySupport => Outer.AnySupport;
            private Func<IEventSourcedContext, TClass> Factory => Outer.Factory;
            private Dictionary<string, IResolvedServiceMethod> ResolvedMethods => Outer.ResolvedMethods;
            private Object[] CurrentBehaviors { get; }
            private string BehaviorsString => CurrentBehaviors?.Aggregate("", (agg, cur) => agg + ", " + cur.GetType()) ?? "";

            public IEventSourcedContext Context { get; }

            public IServiceCallFactory ServiceCallFactory => Context.ServiceCallFactory;

            public EntityHandler(
                AnnotationBasedEventSourcedSupport<TClass> outer,
                IEventSourcedContext context
            )
            {

                Outer = outer;
                Context = context;

                var entity = Factory(context);
                if (entity == null)
                    throw new NullReferenceException($"Entity factory could not instantiate a new instance of [{typeof(TClass)}].");

                CurrentBehaviors = new object[] { entity };

                // TODO: Need to think about this a bit more so I get the context / active flags right..
                // var currentBehaviors = () => {
                //     explicitlySetBehaviors Optional.Option.None<Object[]>();
                //     var active = true;
                //     var isActive = () => active;
                //     var setBehaviors = (behaviors) => {
                //         // currentBehaviors = validateBehaviors(behaviors) // from outer class
                //     };
                //     var ctx = new DelegatingEventSourcedContext(context);

                //     var entity = Factory.Invoke(context);
                //     active = false;
                //     // explicitlySetBehaviors.Match(
                //         // some: x => behaviors
                //         // none: entity
                //     //)
                // };

            }

            public void HandleEvent(Any @event, IEventContext context)
            {

                var behavior = CurrentBehaviors.Take(1).FirstOrDefault();
                if (behavior == null)
                {
                    throw new Exception($"No event handler [{@event.GetType().FullName}] found for any of the current behaviors.");
                }
                var obj = AnySupport.Decode(@event);
                var someHandler = GetCachedBehaviorReflection(behavior)
                    .GetCachedEventHandlerForClass(obj.GetType());
                var handler = someHandler.Match(
                    some: x => x,
                    none: () => throw new NullReferenceException()
                );

                var ctx = new DelegatingEventSourcedContext(context);
                try
                {
                    handler.Invoke(behavior, obj, ctx);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Failed to invoke upon handle event");
                    Console.WriteLine(ex);
                    throw; // ??
                }

            }

            public Option<Any> HandleCommand(Any command, ICommandContext context)
                => Unwrap(() => CurrentBehaviors
                        .Take(1)
                        .Select(behavior =>
                        {
                            var handler = GetCachedBehaviorReflection(behavior).CommandHandlers[context.CommandName];
                            return handler.Invoke(behavior, command, context);
                        })
                        .FirstOrDefault()
                        .Or(() => throw new Exception(
                                $"No command handler found for command [{context.CommandName}] on any of the current behaviors: {BehaviorsString}"
                            )
                        )
                    );

            public void HandleSnapshot(Any snapshot, ISnapshotContext context)
                => Unwrap(() =>
                {
                    var anySnapshot = AnySupport.Decode(snapshot);
                    if (!CurrentBehaviors.Any(behavior =>
                    {
                        var handler = GetCachedBehaviorReflection(behavior)
                            .GetCachedSnapshotHandlerForClass(anySnapshot.GetType());
                        if (!handler.HasValue) return false;
                        //                 var active = true
                        //                 val ctx = new DelegatingEventSourcedContext(context) with SnapshotBehaviorContext {
                        //                   override def become(behavior: AnyRef*): Unit = {
                        //                     if (!active) throw new IllegalStateException("Context is not active!")
                        //                     currentBehaviors = validateBehaviors(behavior)
                        //                   }
                        //                   override def sequenceNumber(): Long = context.sequenceNumber()
                        //                 }
                        //                 handler.invoke(behavior, snapshot, ctx)
                        //                 active = false
                        //                 true
                        return false;
                    }))
                    {
                        throw new Exception(
                            $"No snapshot handler found for snapshot {anySnapshot.GetType()} on any of the current behaviors {BehaviorsString}"
                        );
                    }
                });


            public Option<Any> Snapshot(ISnapshotContext context)
                => Unwrap(() => CurrentBehaviors.Take(1)
                        .Select(behavior => GetCachedBehaviorReflection(behavior)
                            .SnapshotInvoker
                            .Map(x => x.Invoke(behavior, context))
                            .Match(
                                some: invoker => Optional.Option.Some(
                                    Any.Pack(invoker as IMessage)
                                ),
                                none: () => Optional.Option.Some<Any>(
                                    Any.Pack(new Empty() as IMessage)
                                )
                            )
                        )
                        .FirstOrDefault()
                );

            private EventBehaviorReflection GetCachedBehaviorReflection(object behavior) => Outer.GetCachedBehaviorReflection(behavior);

            private TResult Unwrap<TResult>(Func<TResult> func)
            {
                try
                {
                    return func();
                }
                catch (Exception ex)
                {
                    if (ex.InnerException != null)
                        throw ex.InnerException;
                    throw;
                }
            }

            private void Unwrap(Action func)
            {
                try
                {
                    func();
                }
                catch (Exception ex)
                {
                    if (ex.InnerException != null)
                        throw ex.InnerException;
                    throw;
                }
            }
        }

    }

}
