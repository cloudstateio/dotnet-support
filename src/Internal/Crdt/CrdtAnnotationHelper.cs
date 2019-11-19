using System;
using System.Collections.Generic;
using System.Reflection;
using CloudState.CSharpSupport.Exceptions;
using CloudState.CSharpSupport.Interfaces.Crdt;
using CloudState.CSharpSupport.Interfaces.Crdt.Contexts;
using CloudState.CSharpSupport.Interfaces.Crdt.Elements;
using CloudState.CSharpSupport.Reflection.ReflectionHelper;
using Optional;

namespace CloudState.CSharpSupport.Crdt
{
    internal class CrdtAnnotationHelper
    {
        internal static Func<ReflectionHelper.MethodParameter, ReflectionHelper.ParameterHandler<C>> CrdtParameterHandlers<C>() 
            where C : ICrdtContext, ICrdtFactory
        {
            return crdt =>
            {
                if (InjectorMap.TryGetValue(crdt.ParameterType, out var injector))
                    return new CrdtParameterHandler<C, ICrdt, object>(injector, crdt.Method);
                if (crdt.ParameterType == typeof(Option))
                    if (InjectorMap.TryGetValue(crdt.ParameterType.GenericTypeArguments[0], out var injector2))
                        //return new OptionalCrdtParameterHandler(injector2, crdt.Method);
                        throw new NotImplementedException("");
                throw new NotImplementedException("");
            };
        }
        
        private KeyValuePair<Type, CrdtInjector<C, C>> Simple<C>(Func<ICrdtFactory, C> create) where C : ICrdt
        {
            var type = typeof(C);
            // TODO: Null below was identity in scala
            return new KeyValuePair<Type, CrdtInjector<C, C>>(type, new CrdtInjector<C, C>(create, null));
        }
        
        internal class CrdtInjector<C, T>
        {
            public Func<ICrdtFactory, T> Create { get; }
            public Func<C, T> Wrap { get; }
            public Type CrdtClass => typeof(C);

            public CrdtInjector(Func<ICrdtFactory, T> create, Func<C, T> wrap)
            {
                Create = create;
                Wrap = wrap;
            }
        }
        
        private static Dictionary<Type, CrdtInjector<ICrdt, object>> InjectorMap = new Dictionary<Type,CrdtInjector<ICrdt, object>>();

        private CrdtInjector<ICrdt, object> Injector(Type type)
        {
            if (InjectorMap.TryGetValue(type, out var injector))
                return injector;
            throw new InvalidOperationException($"Don't know how to inject CRDT of type {type}");
        }

        internal class OptionalCrdtParameterHandler<C, T> : ReflectionHelper.ParameterHandler<C>
            where C : ICrdtContext, ICrdt
            where T : ICrdt, C
        {
            public CrdtInjector<C, T> Injector { get; }
            public MethodBase Method { get; }
            
            public OptionalCrdtParameterHandler(CrdtInjector<C, T> injector, MethodBase method)
            {
                Injector = injector;
                Method = method;
            }

            public override object Apply(ReflectionHelper.InvocationContext<C> ctx)
            {
                return ctx.Context.State<T>(Injector.CrdtClass).Map(x => Injector.Wrap(x));
            }

        }

        internal class CrdtParameterHandler<C, D, T> : ReflectionHelper.ParameterHandler<C>
            where C : ICrdtContext, ICrdtFactory
            where D : ICrdt
        {
            public CrdtInjector<D, T> Injector { get; }
            public MethodBase Method { get; }

            public CrdtParameterHandler(CrdtAnnotationHelper.CrdtInjector<D, T> injector, MethodBase method)
            {
                Injector = injector;
                Method = method;
            }

//            public override object Apply(ReflectionHelper.InvocationContext<ICrdtContext> ctx)
//            {
//                var state = ctx.Context.State(injector.crdtClass)
//                if (state.isPresent) {
//                    injector.wrap(state.get()).asInstanceOf[AnyRef]
//                } else {
//                    injector.create(ctx.context).asInstanceOf[AnyRef]
//                }
//            }
        }
    }
}