using CloudState.CSharpSupport.Interfaces.Services;
using Google.Protobuf.WellKnownTypes;

namespace CloudState.CSharpSupport.Reflection
{
    internal sealed class ResolvedServiceCall<TInput> : IServiceCall<TInput>
    {

        public IServiceCallRef<TInput> Ref { get; }
        IServiceCallRef IServiceCall.Ref => Ref;
        public Any Message { get; }

        internal ResolvedServiceCall(IServiceCallRef<TInput> @ref, Any message)
        {
            Message = message;
            Ref = @ref;
        }

    }
}