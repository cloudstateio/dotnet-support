using System;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;

namespace CloudState.CSharpSupport.Extensions
{
    internal static class AsyncStreamReaderExtensions
    {
        /// <summary>
        /// Extension method on <see cref="IAsyncStreamReader<T>" /> that provides a 
        /// an iterative closure which is scoped to a <param name="startingSequenceNumber" />
        /// </summary>
        /// <param name="stream">IAsyncStreamReader instance</param>
        /// <param name="startingSequenceNumber">Sequence number to start the iteration from</param>
        /// <param name="action">Action to perform on each iteration</param>
        /// <typeparam name="T">Type of message on the stream</typeparam>
        /// <returns>Async Task</returns>
        public static async Task SelectAsync<T>(this IAsyncStreamReader<T> stream, long startingSequenceNumber, Func<long, T, Task> action)
        {
            var i = startingSequenceNumber;
            while (await stream.MoveNext(default))
            {
                await action(i++, stream.Current);
            }
        }
        
        public static async Task SelectAsync<T>(this IAsyncStreamReader<T> stream, Func<T, Task> action)
        {
            while (await stream.MoveNext(default))
            {
                await action(stream.Current);
            }
        }
    }
}