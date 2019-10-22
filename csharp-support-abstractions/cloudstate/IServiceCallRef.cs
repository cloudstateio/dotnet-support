using System;
using Google.Protobuf;
using Google.Protobuf.Reflection;

namespace io.cloudstate.csharpsupport
{
    public interface IServiceCallRef
    {
        MethodDescriptor Method { get; }
    }

    public interface IServiceCallRef<TInput> : IResolvedServiceMethod
        where TInput : IMessage
    {
        IServiceCall CreateCall(TInput message);

    }
}