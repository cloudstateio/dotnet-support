using System;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;

namespace io.cloudstate.csharpsupport.impl
{
    public static class AsyncStreamReaderExtensions
    {
        public static async Task SelectAsync<T>(this IAsyncStreamReader<T> stream, long startingSequenceNumber, Func<long, T, Task> action)
        {
            var i = startingSequenceNumber;
            while (await stream.MoveNext(default(CancellationToken)))
            {
                await action(i++, stream.Current);
            }
        }
    }
}