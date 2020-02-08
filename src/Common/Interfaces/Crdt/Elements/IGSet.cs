using System.Collections.Generic;

namespace CloudState.CSharpSupport.Interfaces.Crdt.Elements
{
  public interface IGSet<T> : ICrdt, ISet<T> {
    // TODO: Require netcore 3.0
    // new bool Remove(T item) {
    //   throw new InvalidOperationException("Remove is not supported on a Grow-only Set.");
    // }
  }
}
