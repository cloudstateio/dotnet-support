using System.Collections.Generic;
using CloudState.CSharpSupport.Interfaces.Crdt.Elements;

namespace CloudState.CSharpSupport.Crdt
{
  public sealed class LWWRegisterMap<K, V> : AbstractORMapWrapper<K, V, ILWWRegister<V>>, 
    IDictionary<K, V> {

    public LWWRegisterMap(IORMap<K, ILWWRegister<V>> ormap) :base(ormap) {
    
    }

    public override V GetCrdtValue(ILWWRegister<V> crdt) {
      return crdt.Get();
    }

    protected override void SetCrdtValue(ILWWRegister<V> crdt, V value) {
      crdt.Set(value);
    }

    protected override ILWWRegister<V> GetOrUpdateCrdt(K key, V value) {
      return ORMap.GetOrCreate(key, f => f.NewLWWRegister(value));
    }
  }
}

