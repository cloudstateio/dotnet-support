using System;
using Google.Protobuf;
using Google.Protobuf.Reflection;

namespace io.cloudstate.csharpsupport.impl
{
    public sealed class StringPrimitive : Primitive<String>
    {

        public StringPrimitive()
            : base(FieldType.String, "")
        {
        }

        public override void Write(CodedOutputStream stream, string t)
        {
            stream.WriteString(t);
        }

        public override string Read(CodedInputStream stream)
        {
            return stream.ReadString();
        }
    }

}