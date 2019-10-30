using Google.Protobuf;
using Google.Protobuf.Reflection;

namespace CloudState.CSharpSupport.Serialization.Primitives
{
    public sealed class Int64Primitive : Primitive<long>
    {

        public Int64Primitive()
            : base(FieldType.Int64, 0)
        {
        }

        public override void Write(CodedOutputStream stream, long t)
        {
            stream.WriteInt64(t);
        }

        public override long Read(CodedInputStream stream)
        {
            return stream.ReadInt64();
        }
    }

}