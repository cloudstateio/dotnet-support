using CloudState.CSharpSupport.Interfaces.Contexts;

namespace CloudState.CSharpSupport.Interfaces.Crdt.Contexts
{
  public interface ICommandContext : ICrdtContext, ICrdtFactory, IEffectContext, IClientActionContext {
  
    long CommandId { get; }

    string CommandName { get; }

    void Delete();
  }
}
