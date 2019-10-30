using System;

namespace CloudState.CSharpSupport.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface)]
    public class JsonableAttribute : CloudStateAttribute { }
}