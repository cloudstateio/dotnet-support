namespace CloudState.CSharpSupport.Interfaces.Crdt.Elements
{
  public interface IFlag : ICrdt {
    bool Enabled { get; }
    void Enable();
  }
}
