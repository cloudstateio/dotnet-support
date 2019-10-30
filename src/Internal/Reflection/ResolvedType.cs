using System;
using CloudState.CSharpSupport.Reflection.Interfaces;
using Google.Protobuf;

namespace CloudState.CSharpSupport.Reflection
{
    internal abstract class ResolvedType<T> : IResolvedType<T>
    {
        public Type TypeClass => typeof(T);

        public string TypeUrl { get; }

        public abstract T ParseFrom(ByteString bytes);

        public abstract ByteString ToByteString(T value);

        public ResolvedType(string typeUrl)
        {
            TypeUrl = typeUrl;
        }

        ByteString IResolvedType.ToByteString(object value)
        {
            return ((IResolvedType<T>)this).ToByteString((T)value);
        }

        object IResolvedType.ParseFrom(ByteString bytes)
        {
            return ((IResolvedType<T>)this).ParseFrom(bytes);
        }
    }
}