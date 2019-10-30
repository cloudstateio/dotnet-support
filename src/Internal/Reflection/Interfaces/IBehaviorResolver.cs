namespace CloudState.CSharpSupport.Reflection.Interfaces
{
    internal interface IBehaviorResolver
    {
        ReflectionHelper.ReflectionHelper.CommandHandlerInvoker GetCommandHandler(object behavior, string commandName);
    }
}