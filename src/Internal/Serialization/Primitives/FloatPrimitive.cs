using Google.Protobuf;
using Google.Protobuf.Reflection;

namespace CloudState.CSharpSupport.Serialization.Primitives
{

    internal sealed class FloatPrimitive : Primitive<float>
    {

        public FloatPrimitive()
            : base(FieldType.Float, 0f)
        {
        }

        public override void Write(CodedOutputStream stream, float t)
        {
            stream.WriteFloat(t);
        }

        public override float Read(CodedInputStream stream)
        {
            return stream.ReadFloat();
        }
    }

}