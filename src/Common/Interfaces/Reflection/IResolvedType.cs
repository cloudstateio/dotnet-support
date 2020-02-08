using System;
using Google.Protobuf;

namespace CloudState.CSharpSupport.Interfaces.Reflection
{
    public interface IResolvedType
    {
        Type TypeClass { get; }
        string TypeUrl { get; }
        object ParseFrom(ByteString bytes);
        ByteString ToByteString(object value);
    }

    public interface IResolvedType<T> : IResolvedType
    {
        ByteString ToByteString(T value);
    }
}
