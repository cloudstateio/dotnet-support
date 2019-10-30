using System;
using System.Linq;
using System.Reflection;
using CloudState.CSharpSupport.Exceptions;
using CloudState.CSharpSupport.Interfaces.EventSourced.Contexts;
using CloudState.CSharpSupport.Reflection.Interfaces;
using Google.Protobuf.WellKnownTypes;
using Optional;
using Type = System.Type;

namespace CloudState.CSharpSupport.Reflection.ReflectionHelper
{
    internal partial class ReflectionHelper
    {
        internal class CommandHandlerInvoker
        {
            public MethodInfo Method { get; }
            public IResolvedServiceMethod ServiceMethod { get; }
            private string Name => ServiceMethod.Descriptor.FullName;
            private ParameterHandler[] Parameters { get; }

            public CommandHandlerInvoker(
                MethodInfo method,
                IResolvedServiceMethod serviceMethod)
            /* TODO: extraParameters: PartialFunction[MethodParameter, ParameterHandler[CommandContext]] = PartialFunction.empty */
            {

                Method = method;
                ServiceMethod = serviceMethod;

                Parameters = GetParameterHandlers<ICommandContext>(method); // TODO: Extra parameters
                if (Parameters.Where(x => x.GetType().IsInstanceOfType(typeof(MainArgumentParameterHandler))).Count() > 1)
                {
                    throw new CloudStateException("Method has too many main arg parameters");
                }

                foreach (var parameter in Parameters)
                {
                    switch (parameter)
                    {
                        case MainArgumentParameterHandler inClass:
                            if (!inClass.Type.IsAssignableFrom(serviceMethod.InputType.TypeClass))
                                throw new CloudStateException(
                                    $"Incompatible command class {inClass} for command {Name}, expected {serviceMethod.InputType.TypeClass}"
                                );
                            break;
                    }
                }
            }


            private Any Serialize(object result)
            {
                // TODO: Protect from null returns
                return new Any
                {
                    TypeUrl = ServiceMethod.OutputType.TypeUrl,
                    Value = ServiceMethod.OutputType.ToByteString(result)
                };
            }

            private Func<object, Option<Any>> HandleResult()
            {

                if (Method.ReturnType == typeof(void))
                    return _ => Optional.Option.Some(Any.Pack(new Empty()));
                if (Method.ReturnType == typeof(Option<>))
                {
                    VerifyOutputType(GetFirstParameter(Method.ReturnType.GenericTypeArguments[0]));
                    return result =>
                    {
                        var asOptional = result as Option<object>?;
                        if (asOptional != null && asOptional.Value.HasValue)
                        {
                            return Optional.Option.Some(
                                Serialize(asOptional.Value.Match(
                                    some: x => x,
                                    none: () => Optional.Option.Some(Any.Pack(new Empty()))
                                ))
                            );
                        }
                        return Optional.Option.Some(Any.Pack(new Empty()));
                    };
                }
                else
                {
                    VerifyOutputType(Method.ReturnType);
                    return result => Optional.Option.Some(Serialize(result));
                }

            }

            public Option<Any> Invoke(object obj, Any command, ICommandContext context)
            {
                var decodedCommand = ServiceMethod.InputType.ParseFrom(command?.Value);
                var ctx = new InvocationContext(decodedCommand, context);
                var result = Method.Invoke(obj, Parameters.Select(x => x.Apply(ctx)).ToArray());
                return HandleResult()(result);
            }

            private Type GetFirstParameter(Type t)
            {
                if (t.IsGenericType)
                    return GetRawType(t.GenericTypeArguments[0]);
                return typeof(object);
            }

            private Type GetRawType(Type t)
            {
                if (t.IsGenericType)
                {
                    return GetRawType(t);
                }
                if (t.IsClass)
                {
                    return t;
                }
                // Note: Wildcard doesn't exist in C#
                // case wct: WildcardType => getRawType(wct.getUpperBounds.headOption.getOrElse(classOf[Object]))
                return typeof(object);
            }

            private void VerifyOutputType(Type type)
            {
                if (!ServiceMethod.OutputType.TypeClass.IsAssignableFrom(GetRawType(type)))
                {
                    throw new CloudStateException(
                        $"Incompatible return class {type} for command {Name}, expected {ServiceMethod.OutputType.TypeClass}"
                    );
                }
            }

        }
    }
}
