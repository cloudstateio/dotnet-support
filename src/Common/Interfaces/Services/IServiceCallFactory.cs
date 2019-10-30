using System;

namespace CloudState.CSharpSupport.Interfaces.Services
{
    public interface IServiceCallFactory
    {
        IServiceCallRef<T> Lookup<T>(string serviceName, string methodName);
        IServiceCallRef Lookup(string serviceName, string methodName, Type messageType);
    }
}
