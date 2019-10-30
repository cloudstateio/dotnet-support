using CloudState.CSharpSupport.Interfaces.Services;

namespace CloudState.CSharpSupport.Interfaces.Contexts
{
    /// <summary>
    /// Root of all contexts
    /// </summary>
    public interface IContext
    {
        IServiceCallFactory ServiceCallFactory { get; }
    }
}
