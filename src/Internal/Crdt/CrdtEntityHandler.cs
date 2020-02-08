using System;
using System.Collections.Generic;
using Cloudstate.Crdt;
using CloudState.CSharpSupport.Contexts.Interfaces;
using CloudState.CSharpSupport.Crdt.Contexts;
using CloudState.CSharpSupport.Crdt.Elements;
using CloudState.CSharpSupport.Crdt.Interfaces;
using CloudState.CSharpSupport.Interfaces.Crdt;
using CloudState.CSharpSupport.Interfaces.Crdt.Contexts;
using CloudState.CSharpSupport.Interfaces.Crdt.Elements;
using CloudState.CSharpSupport.Interfaces.Services;
using CloudState.CSharpSupport.Reflection.ReflectionHelper;
using CloudState.CSharpSupport.Serialization;
using Google.Protobuf.WellKnownTypes;
using Optional;
using Type = System.Type;

namespace CloudState.CSharpSupport.Crdt
{

    internal static class CrdtStateTransformer
    {
        public static IInternalCrdt Create(CrdtState state, AnySupport anySupport)
        {
            IInternalCrdt crdt = null;
            switch (state.StateCase)
            {
                case CrdtState.StateOneofCase.Gcounter:
                    crdt = new GCounterImpl();
                    break;
                default:
                    throw new NotImplementedException();
            }
            crdt.ApplyState(state);
            return crdt;
        }
//                new GCounterImpl
//                case CrdtState.State.Pncounter(_) =>
//                new PNCounterImpl
//                case CrdtState.State.Gset(_) =>
//                new GSetImpl[Any](anySupport)
//                case CrdtState.State.Orset(_) =>
//                new ORSetImpl[Any](anySupport)
//                case CrdtState.State.Flag(_) =>
//                new FlagImpl
//                case CrdtState.State.Lwwregister(_) =>
//                new LWWRegisterImpl[Any](anySupport)
//                case CrdtState.State.Ormap(_) =>
//                new ORMapImpl[Any, InternalCrdt](anySupport)
//                case CrdtState.State.Vote(_) =>
//                new VoteImpl
//            }
//            crdt.applyState(state.state)
//            crdt
//        }
//
//    }    
    }
    
    internal class CrdtEntityCreationContext : ICrdtEntityCreationContext, IActivatableContext //, ICapturingCrdtFactory, IActivatableContext
    {
        private IActivatableContext _activatableContextImplementation;
        public IServiceCallFactory ServiceCallFactory { get; }
        public string EntityId { get; }
        
        public Option<T> State<T>(Type crdtClass) where T : ICrdt
        {
            throw new NotImplementedException();
        }

        public IGCounter NewGCounter()
        {
            throw new NotImplementedException();
        }

        public IPNCounter NewPNCounter()
        {
            throw new NotImplementedException();
        }

        public IGSet<T> NewGSet<T>()
        {
            throw new NotImplementedException();
        }

        public IORSet<T> NewORSet<T>()
        {
            throw new NotImplementedException();
        }

        public IFlag NewFlag()
        {
            throw new NotImplementedException();
        }

        public ILWWRegister<T> NewLWWRegister<T>(T value)
        {
            throw new NotImplementedException();
        }

        public IORMap<K, V> NewORMap<K, V>() where V : ICrdt
        {
            throw new NotImplementedException();
        }

        public IVote NewVote()
        {
            throw new NotImplementedException();
        }

        public bool Active => _activatableContextImplementation.Active;

        public void CheckActive()
        {
            _activatableContextImplementation.CheckActive();
        }

        public void Deactivate()
        {
            _activatableContextImplementation.Deactivate();
        }
    }
    
    internal class CrdtEntityHandler : ICrdtEntityHandler
    {
        private Type EntityClass { get; }
        private object Entity { get; }
        private IDictionary<string, ReflectionHelper.CommandHandlerInvoker<ICommandContext>> CommandHandlers { get; }
        private IDictionary<string, ReflectionHelper.CommandHandlerInvoker<ICommandContext>> StreamedCommandHandlers { get; }

        public CrdtEntityHandler(Type entityClass, object entity, IDictionary<string, ReflectionHelper.CommandHandlerInvoker<ICommandContext>> commandHandlers, IDictionary<string, ReflectionHelper.CommandHandlerInvoker<ICommandContext>> streamedCommandHandlers)
        {
            EntityClass = entityClass;
            Entity = entity;
            CommandHandlers = commandHandlers;
            StreamedCommandHandlers = streamedCommandHandlers;
        }
        
        public Option<Any> HandleCommand(Any command, ICommandContext context)
        {
            return Unwrap(() =>
            {
                // TODO: Used to be a maybe...
                if (!CommandHandlers.TryGetValue(context.CommandName, out var handler))
                    throw new InvalidOperationException(
                        $"No command handler found for command [{context.CommandName}] on CRDT entity: {EntityClass}");
                return handler.Invoke(Entity, command, context);
            });
        }
        
                
        public Option<Any> HandleStreamedCommand(Any command, IStreamedCommandContext<Any> context)
        {
            return Unwrap(() =>
            {
                if (!StreamedCommandHandlers.TryGetValue(context.CommandName, out var handler))
                    throw new InvalidOperationException(
                        $"No streamed command handler found for command [{context.CommandName}] on CRDT entity: {EntityClass}");
        
                var adaptedContext = new AdaptedStreamedCommandContext(context, handler.ServiceMethod.OutputType);
                return handler.Invoke(Entity, command, adaptedContext);
            });
        }
        
        private static TResult Unwrap<TResult>(Func<TResult> func)
        {
            try
            {
                return func();
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                    throw ex.InnerException;
                throw;
            }
        }
        
    }
}