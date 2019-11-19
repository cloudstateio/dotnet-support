using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cloudstate.Crdt;
using CloudState.CSharpSupport.Contexts;
using CloudState.CSharpSupport.Crdt.Interfaces;
using CloudState.CSharpSupport.Extensions;
using CloudState.CSharpSupport.Interfaces.Contexts;
using CloudState.CSharpSupport.Interfaces.Services;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Optional;

namespace CloudState.CSharpSupport.Crdt.Services
{
    internal class CrdtEntityCollectionService : Cloudstate.Crdt.Crdt.CrdtBase, IContext
    {
        private ILogger<CrdtEntityCollectionService> Logger { get; }
        private ILoggerFactory LoggerFactory { get; }
        private IReadOnlyDictionary<string, ICrdtStatefulService> CrdtStatefulServices { get; }
        private IContext RootContext { get; }
        public IServiceCallFactory ServiceCallFactory => RootContext.ServiceCallFactory;

        public CrdtEntityCollectionService(
            ILoggerFactory loggerFactory, 
            IReadOnlyDictionary<string, ICrdtStatefulService> crdtStatefulServices, 
            IContext context)
        {
            LoggerFactory = loggerFactory;
            CrdtStatefulServices = crdtStatefulServices;
            RootContext = context;
            Logger = LoggerFactory.CreateLogger<CrdtEntityCollectionService>();
        }

        public override async Task handle(IAsyncStreamReader<CrdtStreamIn> requestStream, IServerStreamWriter<CrdtStreamOut> responseStream, ServerCallContext context)
        {
            if (!await requestStream.MoveNext())
                return;

            switch (requestStream.Current.MessageCase)
            {
                case CrdtStreamIn.MessageOneofCase.Command:
                case CrdtStreamIn.MessageOneofCase.Changed:
                case CrdtStreamIn.MessageOneofCase.Deleted:
                case CrdtStreamIn.MessageOneofCase.State:
                case CrdtStreamIn.MessageOneofCase.StreamCancelled:
                case CrdtStreamIn.MessageOneofCase.None:
                    throw new InvalidOperationException(
                        $"First message on entity stream is expected to be 'Init'.  Received: [{requestStream.Current.MessageCase}]"
                    );
                case CrdtStreamIn.MessageOneofCase.Init:
                     var init = requestStream.Current.Init;
                     if (!CrdtStatefulServices.TryGetValue(init.ServiceName, out var crdtService)
                     )
                         throw new InvalidOperationException($"Failed to locate service with name {init.ServiceName}");
                     try
                     {
                         var streamContext = new MessageStreamingContext<CrdtStreamIn, CrdtStreamOut>(requestStream, responseStream, context);

                         await RunEntity(init, crdtService, streamContext);
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

        private async Task RunEntity(CrdtInit init, ICrdtStatefulService service, MessageStreamingContext<CrdtStreamIn, CrdtStreamOut> stream)
        {
            var state = CrdtStateTransformer.Create(init.State, service.AnySupport).Some();
            var runner = new EntityRunner(service, init.EntityId, state);
            
            Task ProcessStream(CrdtStreamIn message)
            {
                switch (message.MessageCase)
                {
                    case CrdtStreamIn.MessageOneofCase.Command:
                        runner.HandleCommand(message.Command);
                        break;

                    case CrdtStreamIn.MessageOneofCase.Changed:
                        runner.HandleDelta(message.Changed);
                        break;
                    
                    case CrdtStreamIn.MessageOneofCase.State:
                        break;
                    case CrdtStreamIn.MessageOneofCase.Deleted:
                        break;
                    case CrdtStreamIn.MessageOneofCase.StreamCancelled:
                        break;
                    case CrdtStreamIn.MessageOneofCase.None:
                    case CrdtStreamIn.MessageOneofCase.Init:
                    default:
                        throw new NotImplementedException("Unknown message case");
                }

                return Task.CompletedTask;
            }

            await stream.Request.SelectAsync(ProcessStream);
        }
    }
}