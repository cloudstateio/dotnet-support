using System;
using System.Collections.Generic;

namespace CloudState.CSharpSupport.Interfaces.Crdt.Elements
{
  public interface IORMap<K, V> : ICrdt, IDictionary<K, V> where V : ICrdt {
    
    V GetOrCreate(K key, Func<ICrdtFactory, V> create);

    new V Remove(K key);
  
    // TODO: Needs C# 3.0
    // default V put(K key, V value) {
    //   throw new UnsupportedOperationException("put is not supported on ORMap, use getOrCreate instead");
    // }
  
  }
}
