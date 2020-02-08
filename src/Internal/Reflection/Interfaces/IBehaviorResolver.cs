using System;
using System.Collections.Generic;

namespace CloudState.CSharpSupport.Reflection.Interfaces
{
    internal interface IBehaviorResolver
    {
        EventBehaviorReflection GetOrAdd(Type type);
    }
}