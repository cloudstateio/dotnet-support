using System;
using System.Reflection;
using io.cloudstate.csharpsupport.eventsourced;
using io.cloudstate.csharpsupport.impl;
using io.cloudstate.csharpsupport.impl.eventsourced;
using static io.cloudstate.csharpsupport.impl.ReflectionHelper;

public class SnapshotInvoker
{

    public MethodInfo Method { get; }
    internal ParameterHandler[] Parameters { get; }

    public SnapshotInvoker(MethodInfo method)
    {
        Method = method;
        Parameters = ReflectionHelper.GetParameterHandlers<SnapshotContext>(method);
        foreach (var parameter in Parameters)
            switch (parameter)
            {
                case MainArgumentParameterHandler mainArg:
                    throw new InvalidEntityConstructorParameterException(mainArg.Type);
                default:
                    break;
            }
    }

    public Object Invoke(Object obj, ISnapshotContext context)
    {
        var ctx = new InvocationContext(obj, context);
        return Method.Invoke(obj, new object[] { }) ?? throw new Exception("Invoke returned null");
    }

}