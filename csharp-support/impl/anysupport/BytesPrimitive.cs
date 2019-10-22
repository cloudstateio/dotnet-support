using Google.Protobuf;
using Google.Protobuf.Reflection;

namespace io.cloudstate.csharpsupport.impl
{
    public sealed class BytesPrimitive : Primitive<ByteString>
    {

        public BytesPrimitive()
            : base(FieldType.Bytes, ByteString.Empty)
        {
        }

        public override void Write(CodedOutputStream stream, ByteString t)
        {
            stream.WriteBytes(t);
        }

        public override ByteString Read(CodedInputStream stream)
        {
            return stream.ReadBytes();
        }
    }

}