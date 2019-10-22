using Google.Protobuf;
using Google.Protobuf.Reflection;

namespace io.cloudstate.csharpsupport.impl
{
    public sealed class BoolPrimitive : Primitive<bool>
    {

        public BoolPrimitive()
            : base(FieldType.Bool, false)
        {
        }

        public override void Write(CodedOutputStream stream, bool t)
        {
            stream.WriteBool(t);
        }

        public override bool Read(CodedInputStream stream)
        {
            return stream.ReadBool();
        }
    }

}