using System.Collections.Generic;
using CloudState.CSharpSupport.Reflection.Interfaces;
using Google.Protobuf.Reflection;

namespace CloudState.CSharpSupport.Interfaces
{
    internal interface IStatefulService
    {
        ServiceDescriptor ServiceDescriptor { get; }
        string StatefulServiceTypeName { get; }
        IReadOnlyDictionary<string, IResolvedServiceMethod> Methods { get; }
        string PersistenceId { get; }
    }
}
