using System;
using Google.Protobuf;
using Google.Protobuf.Reflection;

namespace io.cloudstate.csharpsupport.impl
{
    public sealed class Int32Primitive : Primitive<Int32>
    {

        public Int32Primitive()
            : base(FieldType.Int32, 0)
        {
        }

        public override void Write(CodedOutputStream stream, Int32 t)
        {
            stream.WriteInt32(t);
        }

        public override Int32 Read(CodedInputStream stream)
        {
            return stream.ReadInt32();
        }
    }

}