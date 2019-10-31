using CloudState.CSharpSupport.Serialization;
using CloudState.CSharpSupport.Tests.AttributeBasedEntityFactory.CommandHandler.ResolvedTypes;
using Google.Protobuf.Reflection;
using Google.Protobuf.WellKnownTypes;

namespace CloudState.CSharpSupport.Tests.AttributeBasedEntityFactory.EventHandler.Messages
{
    public static class Event
    {
        public static Any Create(string msg)
        {
            return new AnySupport(new FileDescriptor[] {}).Encode(msg);
        }
    }
}