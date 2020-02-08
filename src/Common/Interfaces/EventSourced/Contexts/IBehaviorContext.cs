namespace CloudState.CSharpSupport.Interfaces.EventSourced.Contexts
{
    public interface IBehaviorContext
    {
        void Become(params object[] behaviors);
    }
}