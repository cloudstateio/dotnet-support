using System;
using Google.Protobuf;
using Google.Protobuf.Reflection;
using Google.Protobuf.WellKnownTypes;
using Type = System.Type;

namespace io.cloudstate.csharpsupport.impl
{
    public sealed class ResolvedServiceMethod<TInput, TOutput> : IResolvedServiceMethod, IServiceCallRef<TInput>
        where TOutput : IMessage
        where TInput : IMessage
    {

        public MethodDescriptor Descriptor { get; }

        public IResolvedType<TInput> InputType { get; }
        IResolvedType IResolvedServiceMethod.InputType => InputType;

        public IResolvedType<TOutput> OutputType { get; }
        IResolvedType IResolvedServiceMethod.OutputType => OutputType;

        public Type MessageType => typeof(TInput);

        public MethodDescriptor Method => Descriptor;
        public String Name => Descriptor.Name;
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

        public IServiceCall CreateCall(TInput message)
        {
            return new ResolvedServiceCall<TInput>(
                this,
                new Any()
                {
                    TypeUrl = InputType.TypeUrl,
                    Value = InputType.ToByteString((TInput)message)
                }
            );
        }
    }

}