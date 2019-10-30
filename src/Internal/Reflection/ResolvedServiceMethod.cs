using System;
using CloudState.CSharpSupport.Interfaces.Services;
using CloudState.CSharpSupport.Reflection.Interfaces;
using Google.Protobuf.Reflection;
using Google.Protobuf.WellKnownTypes;

namespace CloudState.CSharpSupport.Reflection
{
    internal sealed class ResolvedServiceMethod<TInput, TOutput>
            : IResolvedServiceMethod<TInput, TOutput>, IServiceCallRef<TInput>
    {

        public MethodDescriptor Descriptor { get; }

        public IResolvedType<TInput> InputType { get; }
        IResolvedType IResolvedServiceMethod.InputType => InputType;
        public IResolvedType<TOutput> OutputType { get; }
        IResolvedType IResolvedServiceMethod.OutputType => OutputType;

        public MethodDescriptor Method => Descriptor;
        public string Name => Descriptor.Name;

        public bool OutputStreamed => Descriptor.IsServerStreaming;

        public ResolvedServiceMethod(
            MethodDescriptor descriptor,
            IResolvedType<TInput> inputType,
            IResolvedType<TOutput> outputType)
        {
            Descriptor = descriptor;
            InputType = inputType;
            OutputType = outputType;
        }

        public IServiceCall<TInput> CreateCall(TInput message)
        {
            return new ResolvedServiceCall<TInput>(
                this,
                new Any
                {
                    TypeUrl = InputType.TypeUrl,
                    Value = InputType.ToByteString(message)
                }
            );
        }

        IServiceCall IServiceCallRef.CreateCall(object message) => CreateCall((TInput)message);
    }
}