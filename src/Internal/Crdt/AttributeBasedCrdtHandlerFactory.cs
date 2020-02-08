using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CloudState.CSharpSupport.Attributes.Crdt;
using CloudState.CSharpSupport.Exceptions;
using CloudState.CSharpSupport.Interfaces;
using CloudState.CSharpSupport.Interfaces.Crdt;
using CloudState.CSharpSupport.Interfaces.Crdt.Contexts;
using CloudState.CSharpSupport.Reflection;
using CloudState.CSharpSupport.Reflection.Interfaces;
using CloudState.CSharpSupport.Reflection.ReflectionHelper;
using CloudState.CSharpSupport.Serialization;
using Google.Protobuf.Reflection;
using Optional;
using Type = System.Type;

namespace CloudState.CSharpSupport.Crdt
{
    internal class AttributeBasedCrdtHandlerFactory : EntityHandlerFactory<ICrdtEntityCreationContext>, ICrdtEntityHandlerFactory, IResolvedEntityFactory
    {
        internal Dictionary<string, ReflectionHelper.CommandHandlerInvoker<ICommandContext>> StreamedCommandHandlers { get; }

        internal Dictionary<string, ReflectionHelper.CommandHandlerInvoker<ICommandContext>> CommandHandlers { get; }
        
        protected internal AttributeBasedCrdtHandlerFactory(Type initialClass, AnySupport anySupport, Func<ICrdtEntityCreationContext, object> factory, IReadOnlyDictionary<string, IResolvedServiceMethod> resolvedMethods) 
            : base(initialClass, anySupport, factory, resolvedMethods)
        {
            var allMethods = ReflectionHelper.GetAllDeclaredMethods(initialClass);
            var handlers = allMethods.Where(x => x.GetCustomAttribute<CommandHandlerAttribute>() != null)
                .Select(method =>
                {
                    var attribute = method.GetCustomAttribute<CommandHandlerAttribute>();
                    var name = string.IsNullOrEmpty(attribute.Name)
                        ? ReflectionHelper.GetCapitalizedName(method)
                        : attribute.Name;
                    if (!ResolvedMethods.TryGetValue(name, out var serviceMethod))
                        throw new CloudStateException(
                            $"Command handler method [{method.Name}] for command [{name}] found, but the service has no command by that name.");
                    return (method, serviceMethod);
                }).ToArray();
            CommandHandlers = GetHandlers<ICommandContext>(handlers, false);
            StreamedCommandHandlers = GetHandlers<IStreamedCommandContext<ICommandContext>>(handlers, true);
        }
        
        protected internal AttributeBasedCrdtHandlerFactory(Type initialClass, AnySupport anySupport, ServiceDescriptor descriptor, Func<ICrdtEntityCreationContext, object> factory = null) 
            : base(initialClass, anySupport, factory, anySupport.ResolveServiceDescriptor(descriptor))
        {
            var allMethods = ReflectionHelper.GetAllDeclaredMethods(initialClass);
            var handlers = allMethods.Where(x => x.GetCustomAttribute<CommandHandlerAttribute>() != null)
                .Select(method =>
                {
                    var attribute = method.GetCustomAttribute<CommandHandlerAttribute>();
                    var name = string.IsNullOrEmpty(attribute.Name)
                        ? ReflectionHelper.GetCapitalizedName(method)
                        : attribute.Name;
                    if (!ResolvedMethods.TryGetValue(name, out var serviceMethod))
                        throw new CloudStateException(
                            $"Command handler method [{method.Name}] for command [{name}] found, but the service has no command by that name.");
                    return (method, serviceMethod);
                }).ToArray();
            CommandHandlers = GetHandlers<ICommandContext>(handlers, false);
            StreamedCommandHandlers = GetHandlers<IStreamedCommandContext<ICommandContext>>(handlers, true);
        }
        
        public ICrdtEntityHandler CreateEntityHandler(ICrdtEntityCreationContext ctx)
        {
            var entity = Constructor(ctx);
            return new CrdtEntityHandler(
                InitialClass,
                entity,
                CommandHandlers,
                StreamedCommandHandlers);
        }

        private object Constructor(ICrdtEntityCreationContext ctx)
        {
            var constructors = InitialClass.GetConstructors();
            if (constructors.Length > 1) 
                throw new InvalidOperationException($"Only a single constructor is allowed on CRDT entities: {InitialClass}");
            return new EntityConstructorInvoker<ICrdtEntityCreationContext>(constructors[0]).Apply(ctx);
        }
        
        private Dictionary<string, ReflectionHelper.CommandHandlerInvoker<ICommandContext>> GetHandlers<T>(IEnumerable<(MethodInfo method, IResolvedServiceMethod serviceMethod)> handlers, bool streamed) where T : ICommandContext
        {
            return handlers.Where(x => x.serviceMethod.OutputStreamed == streamed)
                .Select(x =>
                    new ReflectionHelper.CommandHandlerInvoker<ICommandContext>(
                        x.method,
                        x.serviceMethod,
                        CrdtAnnotationHelper.CrdtParameterHandlers<ICommandContext>()
                    )
                ).GroupBy(x => x.ServiceMethod.Name)
                .ToDictionary(x => x.Key, x => x.Single());
        }

    }
}