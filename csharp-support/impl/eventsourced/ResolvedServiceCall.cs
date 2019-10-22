using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;

namespace io.cloudstate.csharpsupport.impl
{

    sealed class ResolvedServiceCall<TInput> : IServiceCall<TInput>
        where TInput : IMessage
    {

        public IServiceCallRef<TInput> Ref { get; }
        public Any Message { get; }

        public ResolvedServiceCall(IServiceCallRef<TInput> @ref, Any message)
        {
            Message = message;
            Ref = @ref;
        }

        public IServiceCallRef GetRef() => Ref;

    }

}