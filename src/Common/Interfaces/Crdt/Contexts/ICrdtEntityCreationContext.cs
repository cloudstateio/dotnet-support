using CloudState.CSharpSupport.Interfaces.Contexts;
using CloudState.CSharpSupport.Interfaces.EventSourced.Contexts;

namespace CloudState.CSharpSupport.Interfaces.Crdt.Contexts
{
    public interface ICrdtEntityCreationContext : IEntityCreationContext, ICrdtContext, ICrdtFactory {}
}
