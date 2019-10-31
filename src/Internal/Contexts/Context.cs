using System.Diagnostics.CodeAnalysis;
using CloudState.CSharpSupport.Interfaces.Contexts;
using CloudState.CSharpSupport.Interfaces.Services;

namespace CloudState.CSharpSupport.Contexts
{
    [ExcludeFromCodeCoverage]
    internal class Context : IContext
    {
        public IServiceCallFactory ServiceCallFactory { get; }

        public Context(IServiceCallFactory serviceCallFactory)
        {
            ServiceCallFactory = serviceCallFactory;
        }
    }
}
