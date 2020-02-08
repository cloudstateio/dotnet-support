using System;
using CloudState.CSharpSupport.Interfaces.Services;

namespace CloudState.CSharpSupport.Interfaces.Contexts
{
    public interface IClientActionContext : IContext
    {
        Exception Fail(string errorMessage);

        void Forward(IServiceCall to);
    }
}