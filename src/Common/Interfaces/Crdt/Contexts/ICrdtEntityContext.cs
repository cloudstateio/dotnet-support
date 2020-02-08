using System;
using CloudState.CSharpSupport.Interfaces.Contexts;
using CloudState.CSharpSupport.Interfaces.Crdt.Elements;
using Optional;

namespace CloudState.CSharpSupport.Interfaces.Crdt.Contexts
{
  public interface ICrdtContext : IEntityContext
  {
    Option<T> State<T>(Type crdtClass) where T : ICrdt;
  }
}
