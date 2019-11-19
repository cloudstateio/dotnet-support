using CloudState.CSharpSupport.Interfaces.Contexts;

namespace CloudState.CSharpSupport.Interfaces.Crdt.Contexts
{
  public interface ISubscriptionContext : ICrdtContext, IEffectContext, IClientActionContext { 
    void EndStream();
  }
}
