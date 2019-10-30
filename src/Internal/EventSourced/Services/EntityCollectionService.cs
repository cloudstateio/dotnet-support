using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CloudState.CSharpSupport.Contexts;
using CloudState.CSharpSupport.Contexts.Abstractions;
using CloudState.CSharpSupport.EventSourced.Contexts;
using CloudState.CSharpSupport.EventSourced.Interfaces;
using CloudState.CSharpSupport.Interfaces.Contexts;
using CloudState.CSharpSupport.Interfaces.Services;
using Cloudstate.Eventsourced;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using static Cloudstate.Eventsourced.EventSourcedStreamIn;

namespace CloudState.CSharpSupport.EventSourced.Services
{
    internal class EntityCollectionService : Cloudstate.Eventsourced.EventSourced.EventSourcedBase, IContext
    {
        private ILogger<EntityCollectionService> Logger { get; }
        private ILoggerFactory LoggerFactory { get; }
        private CloudStateWorker.CloudStateConfiguration Config { get; }
        private IReadOnlyDictionary<string, IEventSourcedStatefulService> EventSourcedStatefulServices { get; }
        public IContext RootContext { get; }
        public IServiceCallFactory ServiceCallFactory => RootContext.ServiceCallFactory;

        public EntityCollectionService(ILoggerFactory loggerFactory, CloudStateWorker.CloudStateConfiguration config,
            IReadOnlyDictionary<string, IEventSourcedStatefulService> eventSourcedStatefulServices, Context context)
        {
            LoggerFactory = loggerFactory;
            Logger = LoggerFactory.CreateLogger<EntityCollectionService>();

            Config = config;
            EventSourcedStatefulServices = eventSourcedStatefulServices;
            RootContext = context;
        }

        public override async Task handle(IAsyncStreamReader<EventSourcedStreamIn> requestStream, IServerStreamWriter<EventSourcedStreamOut> responseStream, ServerCallContext context)
        {
            if (!(await requestStream.MoveNext()))
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

        private async Task RunEntity(EventSourcedInit init, IEventSourcedStatefulService statefulService, MessageStreamingContext stream)
        {
            var entityId = init.EntityId;
            var entityHandler = statefulService.CreateEntityHandler(
                new EventSourcedContext(entityId, new AbstractContext(RootContext))
            );

            var sequence = 0L;
            var snapshotEvery = 0;


            // TODO: Apply snapshot

            while (await stream.Request.MoveNext())
            {
                var message = stream.Request.Current;
                switch (message.MessageCase)
                {
                    case MessageOneofCase.Command:
                        var command = message.Command;
                        if (command.EntityId != entityId)
                            throw new InvalidOperationException("Entity received a message was not intended for itself");
                        var activateableContext = new AbstractActivatableContext();
                        var commandContext = new CommandContext(
                            entityId,
                            sequence,
                            command.Name,
                            command.Id,
                            statefulService.AnySupport,
                            entityHandler,
                            snapshotEvery,
                            new AbstractContext(RootContext),
                            new AbstractEffectContext(activateableContext),
                            activateableContext
                        );
                        var commandResult = entityHandler
                            .HandleCommand(message.Command.Payload, commandContext);
                        // TODO: response
                        break;

                    case MessageOneofCase.Event:
                        var @event = message.Event;
                        break;

                    case MessageOneofCase.Init:
                        throw new InvalidOperationException($"Entity [{entityId}] already inited");
                    case MessageOneofCase.None:
                        throw new InvalidOperationException($"Missing message");
                    default:
                        throw new NotImplementedException("Unknown message case");

                }

            }
        }

    }
}
