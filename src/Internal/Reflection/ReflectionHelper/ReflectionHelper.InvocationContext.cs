namespace CloudState.CSharpSupport.Reflection.ReflectionHelper
{
    internal partial class ReflectionHelper
    {
        internal class InvocationContext
        {
            public object MainArgument { get; }
            public object Context { get; }

            internal InvocationContext(object mainArgument, object context)
            {
                MainArgument = mainArgument;
                Context = context;
            }

            internal InvocationContext()
            {

            }
        }

    }
}