using CloudState.CSharpSupport.Reflection;
using CloudState.CSharpSupport.Serialization;
using CloudState.CSharpSupport.Tests.AttributeBasedEntityFactory.CommandHandler.Messages;
using Google.Protobuf;

namespace CloudState.CSharpSupport.Tests.AttributeBasedEntityFactory.CommandHandler.ResolvedTypes
{

    internal class WrappedResolvedType : ResolvedType<Wrapped>
    {
        public WrappedResolvedType()
            : base(AnySupport.DefaultTypeUrlPrefix + "/wrapped")
        {

        }

        public override Wrapped ParseFrom(ByteString bytes) => new Wrapped(bytes.ToStringUtf8());

        public override ByteString ToByteString(Wrapped value) => ByteString.CopyFromUtf8(value.Value);
    }

}
