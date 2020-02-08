using Cloudstate;

namespace CloudState.CSharpSupport.Crdt.Contexts
{
    internal class CrdtCommandContext
    {
        public Command Command { get; }
        public CrdtCommandContext(Command command)
        {
            Command = command;
        }
    }
}