using CloudState.CSharpSupport.Serialization;
using CloudState.CSharpSupport.Tests.AttributeBasedEntityFactory.CommandHandler.ResolvedTypes;
using Google.Protobuf;
using Google.Protobuf.Reflection;
using Google.Protobuf.WellKnownTypes;

namespace CloudState.CSharpSupport.Tests.AttributeBasedEntityFactory.CommandHandler.Messages
{
    public class Command
    {
        public static Any Create(string msg)
        {
            return new Any
            {
                Value = new StringResolvedType().ToByteString(msg),
                TypeUrl = new StringResolvedType().TypeUrl
            };
        }
    }
}