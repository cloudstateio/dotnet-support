using Google.Protobuf.Reflection;

namespace CloudState.CSharpSupport.Interfaces.Services
{
    public interface IServiceCallRef<in T> : IServiceCallRef
    {
        IServiceCall<T> CreateCall(T message);
    }

    public interface IServiceCallRef
    {
        IServiceCall CreateCall(object message);
        MethodDescriptor Method { get; }
    }
}