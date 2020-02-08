using CloudState.CSharpSupport.Crdt.Elements;
using CloudState.CSharpSupport.Crdt.Interfaces;
using CloudState.CSharpSupport.Interfaces.Crdt;
using CloudState.CSharpSupport.Interfaces.Crdt.Elements;
using CloudState.CSharpSupport.Serialization;

namespace CloudState.CSharpSupport.Crdt
{
    internal abstract class AbstractCrdtFactory : ICrdtFactory
    {
        protected AnySupport AnySupport { get; }
        protected abstract TCrdt NewCrdt<TCrdt>(TCrdt crdt) where TCrdt : IInternalCrdt;
        
        public IGCounter NewGCounter()
        {
            return NewCrdt(new GCounterImpl());
        }
        
        
    //        // TODO JavaDoc
    //        override def newPNCounter(): PNCounter = newCrdt(new PNCounterImpl)
    //        // TODO JavaDoc
    //        override def newGSet[T](): GSet[T] = newCrdt(new GSetImpl[T](anySupport))
    //        // TODO JavaDoc
    //        override def newORSet[T](): ORSet[T] = newCrdt(new ORSetImpl[T](anySupport))
    //        // TODO JavaDoc
    //        override def newFlag(): Flag = newCrdt(new FlagImpl)
    //        // TODO JavaDoc
    //        override def newLWWRegister[T](value: T): LWWRegister[T] = {
//            val register = newCrdt(new LWWRegisterImpl[T](anySupport))
//            if (value != null) {
//                register.set(value)
//            }
//            register
//        }
//        // TODO JavaDoc
//        override def newORMap[K, V <: Crdt](): ORMap[K, V] =
//        newCrdt(new ORMapImpl[K, InternalCrdt](anySupport)).asInstanceOf[ORMap[K, V]]
//        // TODO JavaDoc
//        override def newVote(): Vote = newCrdt(new VoteImpl)
//    }
        

        public IPNCounter NewPNCounter()
        {
            throw new System.NotImplementedException();
        }

        public IGSet<T> NewGSet<T>()
        {
            throw new System.NotImplementedException();
        }

        public IORSet<T> NewORSet<T>()
        {
            throw new System.NotImplementedException();
        }

        public IFlag NewFlag()
        {
            throw new System.NotImplementedException();
        }

        public ILWWRegister<T> NewLWWRegister<T>(T value)
        {
            throw new System.NotImplementedException();
        }

        public IORMap<K, V> NewORMap<K, V>() where V : ICrdt
        {
            throw new System.NotImplementedException();
        }

        public IVote NewVote()
        {
            throw new System.NotImplementedException();
        }
    }
}