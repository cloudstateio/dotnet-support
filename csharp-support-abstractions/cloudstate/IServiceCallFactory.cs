using System;
using Google.Protobuf;

namespace io.cloudstate.csharpsupport
{
    public interface IServiceCallFactory
    {
        IServiceCallRef<TInput> Lookup<TInput>(String serviceName, String methodName)
            where TInput : IMessage;
    }
}