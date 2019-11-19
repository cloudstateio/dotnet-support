using CloudState.CSharpSupport.Interfaces.Contexts;

namespace CloudState.CSharpSupport.Interfaces.Crdt.Contexts
{
  public interface IStreamCancelledContext : ICrdtContext, IEffectContext {
    long CommandId();
  }
}
