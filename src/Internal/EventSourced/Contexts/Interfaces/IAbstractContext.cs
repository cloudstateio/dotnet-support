using CloudState.CSharpSupport.Interfaces.Services;

namespace CloudState.CSharpSupport.EventSourced.Contexts.Interfaces
{
    internal interface IAbstractContext
    {
        IServiceCallFactory ServiceCallFactory { get; }
    }
}