using System;
using CloudState.CSharpSupport.Interfaces.Reflection;
using CloudState.CSharpSupport.Reflection.Interfaces;
using Google.Protobuf;

namespace CloudState.CSharpSupport.Reflection
{
    internal abstract class ResolvedType<T> : IResolvedType
    {
        public Type TypeClass => typeof(T);

        public string TypeUrl { get; }

        public abstract T ParseFrom(ByteString bytes);

        public abstract ByteString ToByteString(T value);

        object IResolvedType.ParseFrom(ByteString bytes)
        {
            return this.ParseFrom(bytes);
        }

        ByteString IResolvedType.ToByteString(object value)
        {
            return this.ToByteString((T)value);
        }

        public ResolvedType(string typeUrl)
        {
            TypeUrl = typeUrl;
        }


    }
}