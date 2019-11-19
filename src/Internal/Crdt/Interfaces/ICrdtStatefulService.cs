using CloudState.CSharpSupport.Interfaces;
using CloudState.CSharpSupport.Interfaces.Crdt;
using CloudState.CSharpSupport.Serialization;

namespace CloudState.CSharpSupport.Crdt.Interfaces
{
    internal interface ICrdtStatefulService : IStatefulService
    {
        ICrdtEntityHandlerFactory Factory { get; }
        AnySupport AnySupport { get;}
        bool IsStreamed(string command);
    }
}