using CloudState.CSharpSupport.Serialization.Primitives.Interfaces;
using Google.Protobuf;
using Google.Protobuf.Reflection;
using Type = System.Type;

namespace CloudState.CSharpSupport.Serialization.Primitives
{
    public abstract class Primitive<T> : IPrimitive
    {
        public Type ClassType => typeof(T);
        public string Name => FieldType.ToString().ToLower();
        public string FullName => AnySupport.CloudStatePrimitive + Name;
        public T DefaultValue { get; }
        public FieldType FieldType { get; }
        public uint Tag => (AnySupport.CloudStatePrimitiveFieldNumber << 3) | (uint)FieldType.GetTypeCode();

        public Primitive(FieldType fieldType, T defaultValue)
        {
            FieldType = fieldType;
            DefaultValue = defaultValue;
        }

        public abstract void Write(CodedOutputStream stream, T t);
        public abstract T Read(CodedInputStream stream);
    }

}