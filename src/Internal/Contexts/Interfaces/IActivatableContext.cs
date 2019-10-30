namespace CloudState.CSharpSupport.Contexts.Interfaces
{
    internal interface IActivatableContext
    {
        bool Active { get; }
        void CheckActive();
        void Deactivate();
    }
}
