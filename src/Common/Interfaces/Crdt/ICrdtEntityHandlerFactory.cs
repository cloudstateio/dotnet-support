using CloudState.CSharpSupport.Interfaces.Crdt.Contexts;
using CloudState.CSharpSupport.Interfaces.EventSourced;

namespace CloudState.CSharpSupport.Interfaces.Crdt
{
  public interface ICrdtEntityHandlerFactory : IEntityHandlerFactory<ICrdtEntityCreationContext, ICrdtEntityHandler> {
    
  }
}
