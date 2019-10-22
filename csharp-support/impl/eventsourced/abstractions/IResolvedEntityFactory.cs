using System;
using System.Collections.Generic;
using Google.Protobuf;

namespace io.cloudstate.csharpsupport.impl
{
    public interface IResolvedEntityFactory
    {
        Dictionary<String, IResolvedServiceMethod> ResolvedMethods { get; }
    }
}