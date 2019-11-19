using System;
using System.Collections.Generic;
using CloudState.CSharpSupport.Interfaces.Crdt;
using CloudState.CSharpSupport.Interfaces.Crdt.Elements;

namespace CloudState.CSharpSupport.Crdt
{

  public sealed class PNCounterMap<K> : AbstractORMapWrapper<K, long, IPNCounter>, IDictionary<K, long>
  {

    public PNCounterMap(IORMap<K, IPNCounter> ormap) : base(ormap)
    {

    }

    public long GetValue(Object key)
    {
      IPNCounter counter = ORMap[(K)key];
      if (counter != null)
      {
        return counter.GetValue();
      }
      else
      {
        return 0;
      }
    }

    public long Increment(Object key, long by)
    {
      return GetOrUpdate(key).Increment(by);
    }

    public long Decrement(Object key, long by)
    {
      return GetOrUpdate(key).Decrement(by);
    }

    public long Put(K key, long value)
    {
      throw new InvalidOperationException(
        "Put is not supported on PNCounterMap, use increment or decrement instead");
    }

    public override long GetCrdtValue(IPNCounter pnCounter)
    {
      return pnCounter.GetValue();
    }

    protected override void SetCrdtValue(IPNCounter pnCounter, long value)
    {
      throw new InvalidOperationException(
        "Using value mutating methods on PNCounterMap is not supported, use increment or decrement instead");
    }

    protected override IPNCounter GetOrUpdateCrdt(K key, long value)
    {
      return ORMap.GetOrCreate(key, x => x.NewPNCounter());
    }

    private IPNCounter GetOrUpdate(Object key)
    {
      return ORMap.GetOrCreate((K) key, x => x.NewPNCounter());
    }
  }
}

