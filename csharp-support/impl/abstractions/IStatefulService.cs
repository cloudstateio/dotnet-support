using System;
using System.Collections.Generic;
using Google.Protobuf.Reflection;
using io.cloudstate.csharpsupport.impl;

namespace io.cloudstate.csharpsupport.eventsourced.impl
{
    public interface IStatefulService
    {

        ServiceDescriptor Descriptor { get; }

        String EntityType { get; }

        Dictionary<String, IResolvedServiceMethod> ResolvedMethods { get; }

        String PersistenceId => Descriptor.Name;

    }
}