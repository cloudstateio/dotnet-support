using System;
using Google.Protobuf.Reflection;

namespace io.cloudstate.csharpsupport.impl
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