using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CloudState.CSharpSupport.Attributes.EventSourced;
using CloudState.CSharpSupport.Exceptions;
using CloudState.CSharpSupport.Interfaces.Contexts;
using CloudState.CSharpSupport.Interfaces.EventSourced.Contexts;
using CloudState.CSharpSupport.Reflection.ReflectionHelper;
using Optional;
using Optional.Collections;

namespace CloudState.CSharpSupport.EventSourced.Reflection
{
    internal class SnapshotHandlerInvoker
    {
        internal MethodInfo Method { get; }
        internal Type SnapshotClass { get; }
        internal SnapshotHandlerAttribute Attribute { get; }
        internal ReflectionHelper.ParameterHandler<ISnapshotBehaviorContext>[] Parameters { get; }

        public SnapshotHandlerInvoker(MethodInfo method)
        {
            Method = method;
            Attribute = method.GetCustomAttribute<SnapshotHandlerAttribute>() ?? throw new ArgumentNullException(
                    $"Target snapshot handler method [{method.Name}] is not decorated with [{nameof(SnapshotHandlerAttribute)}]"
                );
            Parameters = ReflectionHelper.GetParameterHandlers<ISnapshotBehaviorContext>(method);
            SnapshotClass = Parameters
                    .Select(_ => (_ as ReflectionHelper.MainArgumentParameterHandler<ISnapshotBehaviorContext>)?.Type)
                    .Where(_ => _ != null)
                    .ToArray() switch
            {
                { } single when single.Length == 1 => single[0],
                { } others => throw new CloudStateException(
                    $"SnapshotHandler method [{method}] must defined at most one non context parameter " +
                    "to handle snapshots, the parameters defined were: " +
                    $"[{string.Join(", ", others.Select(_ => _.Name))}]"),
                _ => throw new InvalidOperationException()
            };

        }

        public void Invoke(object obj, object snapshot, ISnapshotBehaviorContext context)
        {
            var ctx = new ReflectionHelper.InvocationContext<ISnapshotBehaviorContext>(snapshot, context);
            Method.Invoke(obj, Parameters.Select(_ => _.Apply(ctx)).ToArray());
        }
    }
}