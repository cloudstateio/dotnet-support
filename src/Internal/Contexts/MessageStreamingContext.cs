using System.Diagnostics.CodeAnalysis;
using Cloudstate.Eventsourced;
using Grpc.Core;

namespace CloudState.CSharpSupport.Contexts
{

    [ExcludeFromCodeCoverage]
    internal class MessageStreamingContext<TIn, TOut>
    {
        internal IAsyncStreamReader<TIn> Request { get; }
        internal IServerStreamWriter<TOut> Response { get; }
        internal ServerCallContext ServerCallContext { get; }

        internal MessageStreamingContext(IAsyncStreamReader<TIn> requestStream, IServerStreamWriter<TOut> responseStream, ServerCallContext context)
        {
            Request = requestStream;
            Response = responseStream;
            ServerCallContext = context;
        }
    }



}
