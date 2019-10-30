using Google.Protobuf;
using Google.Protobuf.Reflection;

namespace CloudState.CSharpSupport.Serialization.Primitives
{
    public sealed class Int32Primitive : Primitive<int>
    {

        public Int32Primitive()
            : base(FieldType.Int32, 0)
        {
        }

        public override void Write(CodedOutputStream stream, int t)
        {
            stream.WriteInt32(t);
        }

        public override int Read(CodedInputStream stream)
        {
            return stream.ReadInt32();
        }
    }

}