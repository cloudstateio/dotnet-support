using System.Diagnostics.CodeAnalysis;
using Cloudstate.Eventsourced;
using Grpc.Core;

namespace CloudState.CSharpSupport.Contexts
{

    [ExcludeFromCodeCoverage]
    internal class MessageStreamingContext
    {
        internal IAsyncStreamReader<EventSourcedStreamIn> Request { get; }
        internal IServerStreamWriter<EventSourcedStreamOut> Response { get; }
        internal ServerCallContext ServerCallContext { get; }

        internal MessageStreamingContext(IAsyncStreamReader<EventSourcedStreamIn> requestStream, IServerStreamWriter<EventSourcedStreamOut> responseStream, ServerCallContext context)
        {
            Request = requestStream;
            Response = responseStream;
            ServerCallContext = context;
        }
    }



}
