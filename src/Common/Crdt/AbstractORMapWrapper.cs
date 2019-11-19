using System.Collections.Generic;
using CloudState.CSharpSupport.Interfaces.Crdt.Elements;

// TODO: V is nullable.
namespace CloudState.CSharpSupport.Crdt
{
  public abstract class AbstractORMapWrapper<K, V, C> : Dictionary<K, V> 
    where C : ICrdt {

    protected IORMap<K, C> ORMap { get; }

    public new ICollection<K> Keys => ORMap.Keys;

    // TODO: This was supposed to override EntrySet and return a new EntrySet();
    //public IEnumerable<KeyValuePair<K, C>> Entries => ORMap;

    public new int Count => ORMap.Count;

    public new V this[K key] 
    {
      get => Get(key);
      set => Put(key, value);
    }

    protected AbstractORMapWrapper(IORMap<K, C> orMap) {
      ORMap = orMap;
    }

    public abstract V GetCrdtValue(C crdt);

    protected abstract void SetCrdtValue(C crdt, V value);

    protected abstract C GetOrUpdateCrdt(K key, V value);

    public new bool ContainsKey(K key)
    {
      return ORMap.ContainsKey(key);
    }

    public new V Remove(K key) {
      C old = ORMap.Remove(key);
      if (old != null) {
        return GetCrdtValue(old);
      } else {
        // TODO: Needs netcore 3.0
        // return null;
        return default(V);
      }
    }

    public new void Clear() {
      ORMap.Clear();
    }

    private V Get(K key) {
      if (!ORMap.TryGetValue(key, out var crdt)) {
        if (crdt != null)
          return GetCrdtValue(crdt);
      }
      // TODO: Needs netcore 3.0
      // return null;
      return default(V);
    }

    private V Put(K key, V value) {
      if (!ORMap.TryGetValue(key, out var existing)) {
        if (existing != null) {
          V old = GetCrdtValue(existing);
          SetCrdtValue(existing, value);
          return old;
        }
      }
      GetOrUpdateCrdt(key, value);
      // TODO: Needs netcore 3.0
      // return null;
      return default(V);
    }

    // TODO: Override the following.


    // private class MapEntry<K, V> 
    // {
    //   // private final Entry<K, C> entry;

    //   // MapEntry(Entry<K, C> entry) {
    //   //   this.entry = entry;
    //   // }

    //   // @Override
    //   // public K getKey() {
    //   //   return entry.getKey();
    //   // }

    //   // @Override
    //   // public V getValue() {
    //   //   return getCrdtValue(entry.getValue());
    //   // }

    //   // @Override
    //   // public V setValue(V value) {
    //   //   V old = getCrdtValue(entry.getValue());
    //   //   setCrdtValue(entry.getValue(), value);
    //   //   return old;
    //   // }

    //   // public void Add(K key, V value)
    //   // {
    //   //     throw new System.NotImplementedException();
    //   // }

    

    //   // public bool Remove(K key)
    //   // {
    //   //     throw new System.NotImplementedException();
    //   // }

    //   // public bool TryGetValue(K key, out V value)
    //   // {
    //   //     throw new System.NotImplementedException();
    //   // }

    //   // public void Add(KeyValuePair<K, V> item)
    //   // {
    //   //     throw new System.NotImplementedException();
    //   // }

    //   // public void Clear()
    //   // {
    //   //     throw new System.NotImplementedException();
    //   // }

    //   // public bool Contains(KeyValuePair<K, V> item)
    //   // {
    //   //     throw new System.NotImplementedException();
    //   // }

    //   // public void CopyTo(KeyValuePair<K, V>[] array, int arrayIndex)
    //   // {
    //   //     throw new System.NotImplementedException();
    //   // }

    //   // public bool Remove(KeyValuePair<K, V> item)
    //   // {
    //   //     throw new System.NotImplementedException();
    //   // }

    //   // public IEnumerator<KeyValuePair<K, V>> GetEnumerator()
    //   // {
    //   //     throw new System.NotImplementedException();
    //   // }

    //   // IEnumerator IEnumerable.GetEnumerator()
    //   // {
    //   //     throw new System.NotImplementedException();
    //   // }
    // }

    // private final class EntrySet extends AbstractSet<Entry<K, V>> implements Set<Entry<K, V>> {
    //   @Override
    //   public int size() {
    //     return ormap.size();
    //   }

    //   @Override
    //   public Iterator<Entry<K, V>> iterator() {
    //     return new Iterator<Entry<K, V>>() {
    //       private final Iterator<Entry<K, C>> iter = ormap.entrySet().iterator();

    //       @Override
    //       public boolean hasNext() {
    //         return iter.hasNext();
    //       }

    //       @Override
    //       public Entry<K, V> next() {
    //         return new MapEntry(iter.next());
    //       }

    //       @Override
    //       public void remove() {
    //         iter.remove();
    //       }
    //     };
    //   }

    //   @Override
    //   public boolean add(Entry<K, V> kvEntry) {
    //     return !kvEntry.getValue().equals(put(kvEntry.getKey(), kvEntry.getValue()));
    //   }

    //   @Override
    //   public void clear() {
    //     ormap.clear();
    //   }
  }
}
