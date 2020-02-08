using System;
using CloudState.CSharpSupport.Exceptions;
using CloudState.CSharpSupport.Interfaces.Services;

namespace CloudState.CSharpSupport.Interfaces.EventSourced.Contexts
{
    public interface IClientActionContext
    {
        void Forward(IServiceCall to);
        CloudStateException Fail(String errorMessage);
    }
}
