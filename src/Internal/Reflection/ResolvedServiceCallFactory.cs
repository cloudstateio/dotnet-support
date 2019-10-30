using System;
using System.Collections.Generic;
using CloudState.CSharpSupport.Interfaces;
using CloudState.CSharpSupport.Interfaces.Services;

namespace CloudState.CSharpSupport.Reflection
{
    internal class ResolvedServiceCallFactory : IServiceCallFactory
    {
        private IReadOnlyDictionary<string, IStatefulService> Services { get; }

        internal ResolvedServiceCallFactory(IReadOnlyDictionary<string, IStatefulService> services)
        {
            Services = services;
        }

        public IServiceCallRef<TInput> Lookup<TInput>(string serviceName, string methodName)
        {
            throw new NotImplementedException();
        }

        public IServiceCallRef Lookup(string serviceName, string methodName, Type messageType)
        {
            throw new NotImplementedException();
        }
    }
}
