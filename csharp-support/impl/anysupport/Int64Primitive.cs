using System;
using Google.Protobuf;
using Google.Protobuf.Reflection;

namespace io.cloudstate.csharpsupport.impl
{
    public sealed class Int64Primitive : Primitive<Int64>
    {

        public Int64Primitive()
            : base(FieldType.Int64, 0)
        {
        }

        public override void Write(CodedOutputStream stream, Int64 t)
        {
            stream.WriteInt64(t);
        }

        public override Int64 Read(CodedInputStream stream)
        {
            return stream.ReadInt64();
        }
    }

}