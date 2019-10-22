using Google.Protobuf.Reflection;

namespace io.cloudstate.csharpsupport
{
    public interface IResolvedServiceMethod : IServiceCallRef
    {
        string Name { get; }
        MethodDescriptor Descriptor { get; }
        IResolvedType InputType { get; }
        IResolvedType OutputType { get; }
    }
}