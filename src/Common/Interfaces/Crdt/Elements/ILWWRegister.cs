namespace CloudState.CSharpSupport.Interfaces.Crdt.Elements
{
  public interface ILWWRegister<T> : ICrdt {
    T Get();
  
    T Set(T value, Clock clock = Clock.DEFAULT, long customClockValue = 0);
    
  }
  
  public enum Clock {
    DEFAULT,
    REVERSE,
    CUSTOM,
    CUSTOM_AUTO_INCREMENT
  }
}
