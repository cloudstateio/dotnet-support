using System;
using System.Collections.Generic;
using System.Linq;
using Cloudstate;
using Cloudstate.Eventsourced;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using io.cloudstate.csharpsupport.eventsourced.impl;
using io.cloudstate.csharpsupport.impl.eventsourced;
using Microsoft.Extensions.Logging;
using Optional;
using static Cloudstate.Eventsourced.EventSourcedStreamIn;

namespace io.cloudstate.csharpsupport.impl
{

    /// <summary>
    /// stream for a single entity
    /// </summary>
    public sealed class EventSourcedService : EventSourced.EventSourcedBase
    {
        private Configuration Configuration { get; }
        private IDictionary<String, EventSourcedStatefulService> Services { get; }
        private Context RootContext { get; }

        ILogger<EventSourcedService> Logger { get; }

        public EventSourcedService(
            ILoggerFactory loggerFactory,
            Configuration configuration,
            IDictionary<String, IStatefulService> services,
            Context rootContext
        )
        {
            Configuration = configuration;
            Services = services.ToDictionary(
                    x => x.Key,
                    x =>
                    {
                        var esss = (EventSourcedStatefulService)x.Value;
                        if (esss.SnapshotEvery == 0)
                        {
                            return esss.WithSnapshotEvery(Configuration.SnapshotEvery);
                        }
                        return esss;
                    }
                );
            RootContext = rootContext;

            Logger = loggerFactory.CreateLogger<EventSourcedService>();
        }

        public override async global::System.Threading.Tasks.Task handle(IAsyncStreamReader<Cloudstate.Eventsourced.EventSourcedStreamIn> requestStream, IServerStreamWriter<global::Cloudstate.Eventsourced.EventSourcedStreamOut> responseStream, ServerCallContext context)
        {
            Logger.LogInformation("Received call: " + requestStream);

            if (!await requestStream.MoveNext())
                throw new Exception("Failed read first of sequence on stream");
            var current = requestStream.Current;

            if (!current.MessageCase.Equals(MessageOneofCase.Init))
                throw new Exception("Expected Init message");
            var init = requestStream.Current.Init;

            if (!Services.TryGetValue(init.ServiceName, out var service))
                throw new Exception($"Service not found: {init.ServiceName}");
            var handler = service.Factory.Create(
                new EventSourcedContext(init.EntityId, RootContext.ServiceCallFactory)
            );

            var entityId = init.EntityId;

            var startingSequenceNumber = 0L;
            for (var snapshot = init?.Snapshot; snapshot != null;)
                for (var any = snapshot.Snapshot; any != null;)
                {
                    var snapshotSequence = snapshot.SnapshotSequence;
                    var snapshotContext = new SnapshotContext(
                        entityId,
                        snapshotSequence,
                        RootContext.ServiceCallFactory
                    );
                    handler.HandleSnapshot(
                        any,
                        snapshotContext
                    );
                    startingSequenceNumber = snapshotSequence;
                }

            await requestStream.SelectAsync(startingSequenceNumber, async (sequence, message) =>
            {

                switch (message.MessageCase)
                {
                    case MessageOneofCase.Event:
                        var eventContext = new EventContext(entityId, message.Event.Sequence, RootContext.ServiceCallFactory);
                        var eventPayload = message.Event.Payload;
                        handler.HandleEvent(message.Event.Payload, eventContext);
                        await responseStream.WriteAsync(
                            new EventSourcedStreamOut() { /* No body from handled event */ }
                        );
                        break;

                    case MessageOneofCase.Command:
                        if (message.Command.EntityId != entityId)
                            throw new InvalidOperationException(
                                "Received message that was not intended for this entity"
                            );
                        var commandPayload = message.Command.Payload;
                        var commandName = System.Type.GetType(message.Command.Name);
                        var commandContext = new CommandContext(
                            RootContext.ServiceCallFactory,
                            entityId,
                            sequence,
                            message.Command.Name,
                            message.Command.Id,
                            service.AnySupport,
                            handler,
                            service.SnapshotEvery
                        );

                        var reply = Optional.Option.None<Any>();
                        try
                        {
                            // FIXME is this allowed to throw
                            reply = handler.HandleCommand(commandPayload, commandContext);
                        }
                        catch (Exception ex)
                        {
                            switch (ex)
                            {
                                case FailInvokedException failInvoked:
                                    reply = Optional.Option.Some<Any>(Any.Pack(new Empty()));
                                    break;
                                default:
                                    break;
                            }
                        }
                        finally
                        {
                            ((IActivateableContext)commandContext).Deactivate();
                        }

                        var anyResult = reply.Match(
                            some: result => result,
                            none: () => throw new NullReferenceException("Command result was null")
                        );

                        var clientAction = ((IAbstractClientActionContext)commandContext).CreateClientAction(reply, false);

                        EventSourcedReply outReply;
                        if (!((IAbstractClientActionContext)commandContext).HasError)
                        {

                            var endSequenceNumber = sequence + commandContext.Events.Count;
                            Option<Any> snapshot = Optional.Option.None<Any>();
                            if (commandContext.PerformSnapshot)
                            {
                                var s = handler.Snapshot(
                                    new SnapshotContext(
                                        entityId,
                                        endSequenceNumber,
                                        RootContext.ServiceCallFactory
                                    )
                                );
                                if (s.HasValue)
                                    snapshot = s;
                            }

                            outReply = new EventSourcedReply()
                            {
                                CommandId = message.Command.Id,
                                ClientAction = clientAction.Match(
                                    some: x => x,
                                    none: () => throw new NullReferenceException(nameof(clientAction))
                                )
                            };
                            outReply.SideEffects.Add(((IAbstractEffectContext)commandContext).Effects);
                            outReply.Events.Add(commandContext.Events);
                            snapshot.MatchSome(x => outReply.Snapshot = x);

                        }
                        else
                        {

                            outReply = new EventSourcedReply()
                            {
                                CommandId = message.Command.Id,
                                ClientAction = new ClientAction()
                                {
                                    Reply = new Reply()
                                    {
                                        Payload = anyResult
                                    }
                                }
                            };

                        }

                        await responseStream.WriteAsync(
                            new EventSourcedStreamOut()
                            {
                                Reply = outReply
                            }
                        );

                        break;

                    case MessageOneofCase.None:
                        throw new Exception("Empty message received");

                    case MessageOneofCase.Init:
                        throw new Exception("Entity already inited");

                    default:
                        throw new NotImplementedException("Unknown message type");

                }

            });

        }

    }
}