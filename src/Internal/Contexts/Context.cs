using CloudState.CSharpSupport.Interfaces.Contexts;
using CloudState.CSharpSupport.Interfaces.Services;

namespace CloudState.CSharpSupport.Contexts
{
    internal class Context : IContext
    {
        public IServiceCallFactory ServiceCallFactory { get; }

        public Context(IServiceCallFactory serviceCallFactory)
        {
            ServiceCallFactory = serviceCallFactory;
        }
    }
}
