using CloudState.CSharpSupport.Interfaces.Crdt.Elements;

namespace CloudState.CSharpSupport.Interfaces.Crdt
{
  public interface ICrdtFactory {
  
    IGCounter NewGCounter();
    IPNCounter NewPNCounter();
    IGSet<T> NewGSet<T>();
    IORSet<T> NewORSet<T>();
    IFlag NewFlag();
    ILWWRegister<T> NewLWWRegister<T>(T value);
    IORMap<K, V> NewORMap<K, V>() where V : ICrdt;
    IVote NewVote();
  
  }
}
