using System;
using Google.Protobuf.WellKnownTypes;

namespace io.cloudstate.csharpsupport.impl
{
    /// <summary>
    /// Extensions to help with the Any ser/deser process
    /// </summary>
    public static class AnyExtensions
    {
        /// <summary>
        /// Easily unpack without having strong type reference
        /// </summary>
        /// <param name="any"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static object Unpack(this Any any, System.Type type)
        {
            var unpackMethod = typeof(Any).GetMethod("Unpack")?.MakeGenericMethod(type)
                ?? throw new CloudStateException("Reflection for Unpack method on Any returned null reference.");
            var cmd = unpackMethod.Invoke(any, new object[] { });
            if (null == cmd)
                throw new CloudStateException(
                    $"Unpacking the command to type {type.Name} resulted in a null reference"
                );
            return cmd;
        }
    }

}