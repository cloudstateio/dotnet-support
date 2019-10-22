using System;
using Google.Protobuf;

namespace io.cloudstate.csharpsupport
{
    public interface IResolvedType
    {
        Type TypeClass { get; }
        string TypeUrl { get; }
        MessageParser Parser { get; }
        object ParseFrom(ByteString bytes);
        ByteString ToByteString(object value);
    }

    public interface IResolvedType<T> : IResolvedType where T : IMessage
    {
        new T ParseFrom(ByteString bytes);
        ByteString ToByteString(T value);
    }


}