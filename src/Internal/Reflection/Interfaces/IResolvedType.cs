using System;
using Google.Protobuf;

namespace CloudState.CSharpSupport.Reflection.Interfaces
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
        new T ParseFrom(ByteString bytes);
        ByteString ToByteString(T value);
    }
}
