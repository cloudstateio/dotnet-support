using CloudState.CSharpSupport.Interfaces.Crdt.Contexts;
using Google.Protobuf.WellKnownTypes;
using Optional;

namespace CloudState.CSharpSupport.Interfaces.Crdt
{
    public interface ICrdtEntityHandler : IEntityHandler {
        Option<Any> HandleCommand(Any command, ICommandContext entityContext);
        Option<Any> HandleStreamedCommand(Any command, IStreamedCommandContext<Any> entityContext);
    }
}