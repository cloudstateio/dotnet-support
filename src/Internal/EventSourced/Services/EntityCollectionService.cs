using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cloudstate;
using CloudState.CSharpSupport.Contexts;
using CloudState.CSharpSupport.Contexts.Abstractions;
using CloudState.CSharpSupport.EventSourced.Contexts;
using CloudState.CSharpSupport.EventSourced.Interfaces;
using CloudState.CSharpSupport.Exceptions;
using CloudState.CSharpSupport.Interfaces.Contexts;
using CloudState.CSharpSupport.Interfaces.Services;
using Cloudstate.Eventsourced;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Optional;
using Optional.Unsafe;
using static Cloudstate.Eventsourced.EventSourcedStreamIn;

namespace CloudState.CSharpSupport.EventSourced.Services
{
    internal class EntityCollectionService : Cloudstate.Eventsourced.EventSourced.EventSourcedBase, IContext
    {
        private ILogger<EntityCollectionService> Logger { get; }
        private ILoggerFactory LoggerFactory { get; }
        private CloudStateWorker.CloudStateConfiguration Config { get; }
        private IReadOnlyDictionary<string, IEventSourcedStatefulService> EventSourcedStatefulServices { get; }
        private IContext RootContext { get; }
        public IServiceCallFactory ServiceCallFactory => RootContext.ServiceCallFactory;

        public EntityCollectionService(ILoggerFactory loggerFactory, CloudStateWorker.CloudStateConfiguration config,
            IReadOnlyDictionary<string, IEventSourcedStatefulService> eventSourcedStatefulServices, IContext context)
        {
            LoggerFactory = loggerFactory;
            Logger = LoggerFactory.CreateLogger<EntityCollectionService>();

            Config = config;
            EventSourcedStatefulServices = eventSourcedStatefulServices;
            RootContext = context;
        }

        public override async Task handle(IAsyncStreamReader<EventSourcedStreamIn> requestStream, IServerStreamWriter<EventSourcedStreamOut> responseStream, ServerCallContext context)
        {
            if (!await requestStream.MoveNext())
                return;

            switch (requestStream.Current.MessageCase)
            {
                case MessageOneofCase.Command:
                case MessageOneofCase.Event:
                case MessageOneofCase.None:
                    throw new InvalidOperationException(
                        $"First message on entity stream is expected to be 'Init'.  Received: [{requestStream.Current.MessageCase}]"
                    );
                case MessageOneofCase.Init:
                    var init = requestStream.Current.Init;
                    if (!EventSourcedStatefulServices.TryGetValue(init.ServiceName, out var eventSourcedStatefulService))
                        throw new InvalidOperationException($"Failed to locate service with name {init.ServiceName}");
                    try
                    {
                        var streamContext = new MessageStreamingContext(requestStream, responseStream, context);
                        await RunEntity(init, eventSourcedStatefulService, streamContext);
                    }
                    catch (Exception ex)
                    {
                        Logger.LogError(ex, "Stream processing failed for entity.");
                        // TODO: translate to response error
                        throw;
                    }
                    break;
                default:
                    throw new InvalidOperationException(
                        $"First message on entity stream is expected to be 'Init'.  Received unknown case."
                    );
            }

        }

        private async Task RunEntity(EventSourcedInit init, IEventSourcedStatefulService statefulService,
            MessageStreamingContext stream)
        {
            var entityId = init.EntityId;
            var entityHandler = statefulService.CreateEntityHandler(
                new EventSourcedContext(entityId, new AbstractContext(RootContext))
            );

            var startingSequenceNumber = 0L;
            var any = init?.Snapshot?.Snapshot;
            if (any != null)
            {
                var snapshotSequence = init.Snapshot.SnapshotSequence;
                var snapshotContext = new SnapshotContext(
                    entityId,
                    snapshotSequence,
                    RootContext.ServiceCallFactory
                );
                entityHandler.HandleSnapshot(
                    any,
                    snapshotContext
                );
                startingSequenceNumber = snapshotSequence;
            }

            await stream.Request.SelectAsync(startingSequenceNumber, async (sequence, message) =>
            {
                while (await stream.Request.MoveNext())
                {
                    switch (message.MessageCase)
                    {
                        case MessageOneofCase.Command:
                            var command = message.Command;
                            if (command.EntityId != entityId)
                                throw new InvalidOperationException(
                                    "Entity received a message was not intended for itself");
                            var activatableContext = new AbstractActivatableContext();
                            var commandContext = new CommandContext(
                                entityId,
                                sequence,
                                command.Name,
                                command.Id,
                                statefulService.AnySupport,
                                entityHandler,
                                statefulService.SnapshotEvery,
                                new AbstractContext(RootContext),
                                new AbstractClientActionContext(RootContext, activatableContext),
                                new AbstractEffectContext(activatableContext),
                                activatableContext
                            );
                            var reply = Optional.Option.None<Any>();
                            try
                            {
                                // FIXME is this allowed to throw
                                reply = entityHandler.HandleCommand(command.Payload, commandContext);
                            }
                            catch (Exception ex)
                            {
                                switch (ex)
                                {
                                    case FailInvokedException _:
                                        reply = Optional.Option.Some(Any.Pack(new Empty()));
                                        break;
                                }
                            }
                            finally
                            {
                                activatableContext.Deactivate();
                            }
                                
                            var anyResult = reply.ValueOr(()=>throw new InvalidOperationException("Command result was null")); 
                            var clientAction = commandContext.AbstractClientActionContext.CreateClientAction(reply, false);

                            EventSourcedReply outReply;
                            if (!commandContext.AbstractClientActionContext.HasError)
                            {
                                var endSequenceNumber = sequence + commandContext.Events.Count;
                                var snapshot = Optional.Option.None<Any>();
                                if (commandContext.PerformSnapshot)
                                {
                                    var s = entityHandler.Snapshot(
                                        new SnapshotContext(
                                            entityId,
                                            endSequenceNumber,
                                            RootContext.ServiceCallFactory
                                        )
                                    );
                                    if (s.HasValue)
                                        snapshot = s;
                                }

                                outReply = new EventSourcedReply
                                {
                                    CommandId = message.Command.Id,
                                    ClientAction = clientAction.ValueOr(() => throw new ArgumentNullException(nameof(clientAction)))
                                };
                                outReply.SideEffects.Add(commandContext.AbstractEffectContext.SideEffects);
                                outReply.Events.Add(commandContext.Events); // UNSAFE
                                snapshot.MatchSome(x => outReply.Snapshot = x);
                            }
                            else
                            {
                                outReply = new EventSourcedReply
                                {
                                    CommandId = message.Command.Id,
                                    ClientAction = new ClientAction
                                    {
                                        Reply = new Reply { Payload = anyResult }
                                    }
                                };

                            }

                            await stream.Response.WriteAsync(
                                new EventSourcedStreamOut { Reply = outReply }
                            );
                            break;

                        case MessageOneofCase.Event:
                            var eventContext = new EventContext(entityId, message.Event.Sequence, new AbstractContext(RootContext));
                            entityHandler.HandleEvent(message.Event.Payload, eventContext);
                            await stream.Response.WriteAsync(
                                new EventSourcedStreamOut { /* No body from handled event */ }
                            );
                            break;

                        case MessageOneofCase.Init:
                            throw new InvalidOperationException($"Entity [{entityId}] already inited");
                        case MessageOneofCase.None:
                            throw new InvalidOperationException($"Missing message");
                        default:
                            throw new NotImplementedException("Unknown message case");

                    }

                }
                
            });
        }
    }
}
