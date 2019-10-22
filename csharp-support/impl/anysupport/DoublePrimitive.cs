using Google.Protobuf;
using Google.Protobuf.Reflection;

namespace io.cloudstate.csharpsupport.impl
{
    public sealed class DoublePrimitive : Primitive<double>
    {

        public DoublePrimitive()
            : base(FieldType.Double, 0f)
        {
        }

        public override void Write(CodedOutputStream stream, double t)
        {
            stream.WriteDouble(t);
        }

        public override double Read(CodedInputStream stream)
        {
            return stream.ReadDouble();
        }
    }

}