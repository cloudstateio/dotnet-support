﻿using CloudState.CSharpSupport.Interfaces.Reflection;
using CloudState.CSharpSupport.Interfaces.Services;
using Google.Protobuf.Reflection;

namespace CloudState.CSharpSupport.Reflection.Interfaces
{
    internal interface IResolvedServiceMethod : IServiceCallRef
    {
        string Name { get; }
        MethodDescriptor Descriptor { get; }
        bool OutputStreamed { get; }
        IResolvedType InputType { get; }
        IResolvedType OutputType { get; }
    }

    internal interface IResolvedServiceMethod<TInput, TOutput> : IResolvedServiceMethod
    {
        new IResolvedType InputType { get; }
        new IResolvedType OutputType { get; }
    }


}
