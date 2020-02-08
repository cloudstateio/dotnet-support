using CloudState.CSharpSupport.EventSourced.Contexts.Interfaces;
using CloudState.CSharpSupport.Interfaces.Contexts;
using CloudState.CSharpSupport.Interfaces.Services;

namespace CloudState.CSharpSupport.EventSourced.Contexts
{
    internal class AbstractContext : IAbstractContext
    {
        private IContext RootContext { get; }
        public IServiceCallFactory ServiceCallFactory => RootContext.ServiceCallFactory;

        public AbstractContext(IContext rootContext)
        {
            RootContext = rootContext;
        }
    }
}
