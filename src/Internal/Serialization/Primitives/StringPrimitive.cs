using Google.Protobuf;
using Google.Protobuf.Reflection;

namespace CloudState.CSharpSupport.Serialization.Primitives
{
    public sealed class StringPrimitive : Primitive<string>
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