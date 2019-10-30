using CloudState.CSharpSupport.Interfaces.Services;

namespace CloudState.CSharpSupport.Interfaces.Contexts
{
    public interface IContext
    {
        // TODO: See if we can make this a bit more internal...
        IServiceCallFactory ServiceCallFactory { get; }
    }
}
