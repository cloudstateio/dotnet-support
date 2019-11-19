namespace CloudState.CSharpSupport.Interfaces.Crdt.Elements
{
  public interface IPNCounter : ICrdt {
    long GetValue();
    long Increment(long by);
    long Decrement(long by);
  }
}
