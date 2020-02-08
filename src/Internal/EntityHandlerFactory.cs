using System;
using System.Collections.Generic;
using System.Linq;
using CloudState.CSharpSupport.Exceptions;
using CloudState.CSharpSupport.Interfaces.Contexts;
using CloudState.CSharpSupport.Reflection;
using CloudState.CSharpSupport.Reflection.Interfaces;
using CloudState.CSharpSupport.Serialization;

namespace CloudState.CSharpSupport
{
    internal class EntityHandlerFactory<TContext> 
        where TContext : IEntityCreationContext
    {
        protected Type InitialClass { get; }
        protected AnySupport AnySupport { get; }
        protected Func<TContext, object> Factory { get; }
        public IReadOnlyDictionary<string, IResolvedServiceMethod> ResolvedMethods { get; }

        protected EntityHandlerFactory(Type initialClass, AnySupport anySupport, Func<TContext, object> factory,
            IReadOnlyDictionary<string, IResolvedServiceMethod> resolvedMethods)
        {
            InitialClass = initialClass;
            AnySupport = anySupport;
            Factory = Constructor(InitialClass, factory);
            ResolvedMethods = resolvedMethods;
        }

        protected Func<TContext, object> Constructor(Type entityClass, Func<TContext, object> factory)
        {
            var entityConstructors = entityClass.GetConstructors();
            if (entityConstructors.Length > 1)
                throw new MultipleEntityConstructorsFoundException(entityClass, GetType());
            var entityConstructor = entityConstructors.First();
            return factory ?? (context => new EntityConstructorInvoker<TContext>(entityConstructor).Apply(context));
        }
    }
}