namespace CloudState.CSharpSupport.Interfaces.Crdt.Elements
{
  public interface IGCounter : ICrdt {
    long Value { get; }
    long Increment(long by);
  }
}
