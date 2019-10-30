using Google.Protobuf.WellKnownTypes;

namespace CloudState.CSharpSupport.Interfaces.Services
{
    public interface IServiceCall<in T> : IServiceCall
    {
        new IServiceCallRef<T> Ref { get; }

    }

    public interface IServiceCall
    {
        IServiceCallRef Ref { get; }
        Any Message { get; }
    }
}
