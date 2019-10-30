namespace CloudState.CSharpSupport.Interfaces.EventSourced.Contexts
{
    public interface ICommandContext : IEventSourcedContext, IClientActionContext, IEffectContext
    {
        string CommandName { get; }
        long Sequence { get; }
        long CommandId { get; }
        void Emit(object @event);
    }
}
