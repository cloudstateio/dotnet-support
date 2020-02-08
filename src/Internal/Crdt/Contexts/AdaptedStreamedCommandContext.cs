using System;
using CloudState.CSharpSupport.Interfaces.Crdt;
using CloudState.CSharpSupport.Interfaces.Crdt.Contexts;
using CloudState.CSharpSupport.Interfaces.Crdt.Elements;
using CloudState.CSharpSupport.Interfaces.Reflection;
using CloudState.CSharpSupport.Interfaces.Services;
using CloudState.CSharpSupport.Reflection.Interfaces;
using Google.Protobuf.WellKnownTypes;
using Optional;
using Type = System.Type;

namespace CloudState.CSharpSupport.Crdt.Contexts
{
    internal class AdaptedStreamedCommandContext : IStreamedCommandContext<object>
    {
        public IStreamedCommandContext<Any> Delegate { get; }
        public IResolvedType ServiceMethodOutputType { get; }
        public long CommandId { get; }
        public string CommandName { get; }
        public string EntityId { get; }
        
        public IServiceCallFactory ServiceCallFactory => Delegate.ServiceCallFactory;
        public bool IsStreamed => Delegate.IsStreamed;

        
        public AdaptedStreamedCommandContext(IStreamedCommandContext<Any> @delegate, IResolvedType serviceMethodOutputType)
        {
            Delegate = @delegate;
            ServiceMethodOutputType = serviceMethodOutputType;
        }

        
        public void OnChange(Func<ISubscriptionContext, Option<object>> subscriber)
        {
            throw new NotImplementedException();
        }

        public void OnCancel(Action<IStreamCancelledContext> effect)
        {
            throw new NotImplementedException();
        }
        
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

        public void Effect(IServiceCall effect, bool synchronous = false)
        {
            throw new NotImplementedException();
        }

        public Exception Fail(string errorMessage)
        {
            throw new NotImplementedException();
        }

        public void Forward(IServiceCall to)
        {
            throw new NotImplementedException();
        }

        public void Delete()
        {
            throw new NotImplementedException();
        }
    }
}