using System.Collections.Generic;
using CloudState.CSharpSupport.Interfaces.Services;

namespace CloudState.CSharpSupport.Interfaces.Contexts
{
    public interface IEffectContext
    {
        void Effect(IServiceCall effect, bool synchronous = false);
    }
}