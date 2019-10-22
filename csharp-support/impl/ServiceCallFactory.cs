using System;
using System.Collections.Generic;
using Google.Protobuf;
using io.cloudstate.csharpsupport.eventsourced.impl;

namespace io.cloudstate.csharpsupport.impl
{

    public class ResolvedServiceCallFactory : IServiceCallFactory
    {

        IDictionary<String, IStatefulService> Services { get; }

        public ResolvedServiceCallFactory(IDictionary<String, IStatefulService> services)
        {
            Services = services;
        }

        public IServiceCallRef<TMessage> Lookup<TMessage>(string serviceName, string methodName)
            where TMessage : IMessage
        {
            var methodType = typeof(TMessage);
            if (Services.TryGetValue(serviceName, out var service))
                if (service.ResolvedMethods.TryGetValue(methodName, out var method))
                    if (method.Method.InputType.ClrType.IsAssignableFrom(methodType))
                        return (IServiceCallRef<TMessage>)method;
                    else
                        throw new ArgumentException(
                            $"The input type {method.Method.InputType.ClrType.Name} of {serviceName}.{methodName} " +
                            "does not match the requested message type {methodType.Name}"
                        );
                else
                    throw new KeyNotFoundException(
                        $"No method named {methodName} found on service {serviceName}"
                    );
            else
                throw new KeyNotFoundException(
                    $"No service named {serviceName} is being handled by this stateful service"
                );
        }
    }
}