namespace io.cloudstate.csharpsupport
{
    public interface IEffectContext : IContext
    {

        void Effect(IServiceCall effect)
        {
            this.Effect(effect, false);
        }

        void Effect(IServiceCall effect, bool synchronous);

    }
}