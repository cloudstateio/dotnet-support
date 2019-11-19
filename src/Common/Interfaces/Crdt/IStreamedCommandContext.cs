using System;
using CloudState.CSharpSupport.Interfaces.Crdt.Contexts;
using Optional;

namespace CloudState.CSharpSupport.Interfaces.Crdt
{
  public interface IStreamedCommandContext<TOutput> : ICommandContext {
    bool IsStreamed { get; }

    void OnChange(Func<ISubscriptionContext, Option<TOutput>> subscriber);
  
    void OnCancel(Action<IStreamCancelledContext> effect);
  }
}
