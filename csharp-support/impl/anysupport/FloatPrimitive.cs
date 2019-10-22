using Google.Protobuf;
using Google.Protobuf.Reflection;

namespace io.cloudstate.csharpsupport.impl
{
    public sealed class FloatPrimitive : Primitive<float>
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