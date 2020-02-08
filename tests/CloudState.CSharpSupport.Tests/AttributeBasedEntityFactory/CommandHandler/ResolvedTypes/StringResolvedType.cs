using CloudState.CSharpSupport.Reflection;
using CloudState.CSharpSupport.Serialization;
using Google.Protobuf;

namespace CloudState.CSharpSupport.Tests.AttributeBasedEntityFactory.CommandHandler.ResolvedTypes
{

    internal class StringResolvedType : ResolvedType<string>
    {
        public StringResolvedType()
            : base(AnySupport.DefaultTypeUrlPrefix + "/string")
        {

        }

        public override string ParseFrom(ByteString bytes) => bytes.ToStringUtf8();

        public override ByteString ToByteString(string value) => ByteString.CopyFromUtf8(value);
    }


}
