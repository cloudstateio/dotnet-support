using System.Collections.Generic;
using System.Linq;
using Cloudstate;
using CloudState.CSharpSupport.Contexts.Interfaces;
using CloudState.CSharpSupport.Interfaces.Contexts;
using CloudState.CSharpSupport.Interfaces.Services;
using Google.Protobuf.WellKnownTypes;

namespace CloudState.CSharpSupport.Contexts.Abstractions
{
    internal class AbstractEffectContext : IEffectContext
    {
        private List<SideEffect> Effects { get; } = new List<SideEffect>();
        private IActivatableContext ActivatableContext { get; }
        public List<SideEffect> SideEffects => Enumerable.Reverse(Effects).ToList();

        public AbstractEffectContext(IActivatableContext activatableContext)
        {
            ActivatableContext = activatableContext;
        }

        public void Effect(IServiceCall effect, bool synchronous = false)
        {
            ActivatableContext.CheckActive();
            Effects.Add(
                new SideEffect
                {
                    ServiceName = effect.Ref.Method.Service.FullName,
                    CommandName = effect.Ref.Method.Name,
                    Payload = Any.Pack(effect.Message),
                    Synchronous = synchronous
                }
            );
        }
    }
}
