using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;

namespace io.cloudstate.csharpsupport.impl
{
    public class CSharpResolvedType<TInput> : IResolvedType<TInput>
        where TInput : IMessage
    {
        public System.Type TypeClass { get; } = typeof(TInput);
        public string TypeUrl { get; }
        public MessageParser Parser { get; }

        public CSharpResolvedType(string typeUrl, MessageParser parser)
        {
            TypeUrl = typeUrl;
            Parser = parser;
        }

        public TInput ParseFrom(ByteString bytes)
        {
            return (TInput)Parser.ParseFrom(
                // NOTE: Might be a better way to coalesce this.  Couldn't see a way in base code..
                bytes ?? new Empty().ToByteString()
            );
        }
        object IResolvedType.ParseFrom(ByteString bytes)
        {
            return ((IResolvedType<TInput>)this).ParseFrom(
                bytes
            );
        }

        public ByteString ToByteString(TInput value)
        {
            return value.ToByteString();
        }

        ByteString IResolvedType.ToByteString(object value)
        {
            return (value as IMessage).ToByteString();
        }
    }

}