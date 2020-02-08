using CloudState.CSharpSupport.Reflection;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;

namespace CloudState.CSharpSupport.Serialization
{
    internal class CSharpResolvedType<TInput> : ResolvedType<TInput>
        where TInput : IMessage
    {

        protected MessageParser Parser { get; }

        public CSharpResolvedType(string typeUrl, MessageParser parser)
            : base(typeUrl)
        {
            Parser = parser;
        }

        public override TInput ParseFrom(ByteString bytes)
        {
            return (TInput)Parser.ParseFrom(
                // NOTE: Might be a better way to coalesce this.  Couldn't see a way in base code..
                bytes ?? new Empty().ToByteString()
            );
        }

        public override ByteString ToByteString(TInput value)
        {
            return value.ToByteString();
        }
    }

}