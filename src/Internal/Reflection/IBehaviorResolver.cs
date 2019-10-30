namespace CloudState.CSharpSupport.Reflection
{
    internal interface IBehaviorResolver
    {
        ReflectionHelper.ReflectionHelper.CommandHandlerInvoker GetCommandHandler(object behavior, string commandName);
    }
}