using System.Collections.Generic;
using System.Linq;
using Cloudstate;
using Google.Protobuf.WellKnownTypes;

namespace io.cloudstate.csharpsupport
{
    public interface IAbstractEffectContext : IEffectContext
    {
        List<SideEffect> Effects => new List<SideEffect>();

        new void Effect(IServiceCall effect, bool synchronous)
        {
            ((IActivateableContext)this).CheckActive();
            Effects.Add(
                new SideEffect()
                {
                    ServiceName = effect.GetRef().Method.Service.FullName,
                    CommandName = effect.GetRef().Method.Name,
                    Payload = Any.Pack(effect.Message),
                    Synchronous = synchronous
                }
            );
        }
    }
}