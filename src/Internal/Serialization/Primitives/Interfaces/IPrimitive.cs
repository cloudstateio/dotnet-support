using System;
using Google.Protobuf.Reflection;

namespace CloudState.CSharpSupport.Serialization.Primitives.Interfaces
{
    public interface IPrimitive
    {
        Type ClassType { get; }
        string Name { get; }
        string FullName { get; }
        FieldType FieldType { get; }
        uint Tag { get; }
    }

}