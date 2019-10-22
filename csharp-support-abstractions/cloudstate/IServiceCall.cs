using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;

namespace io.cloudstate.csharpsupport
{
    public interface IServiceCall<TInput> : IServiceCall
        where TInput : IMessage
    {
        IServiceCallRef<TInput> Ref { get; }
    }

    public interface IServiceCall
    {
        Any Message { get; }
        IServiceCallRef GetRef();

    }
}