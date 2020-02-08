using System;
using Cloudstate.Crdt;
using CloudState.CSharpSupport.Crdt.Interfaces;
using CloudState.CSharpSupport.Interfaces.Crdt.Elements;
using Optional;

namespace CloudState.CSharpSupport.Crdt.Elements
{
    internal class GCounterImpl : IInternalCrdt, IGCounter
    {
        private long DeltaValue { get; set; } = 0;

        public string Name => "GCounter";
        public long Value { get; private set; } = 0;
        public bool HasDelta => DeltaValue != 0;

        public Option<CrdtDelta> Delta => HasDelta
            ? new CrdtDelta { Gcounter = new GCounterDelta() }.Some()
            : Option.None<CrdtDelta>();

        public void ResetDelta()
        {
            DeltaValue = 0;
        }

        public CrdtState State => new CrdtState { Gcounter = new GCounterState {Value = (ulong)Value} };

        public Action<CrdtDelta> ApplyDelta => (d) => {
            switch (d.DeltaCase)
            {
                case CrdtDelta.DeltaOneofCase.Gcounter:
                    Value += (long)d.Gcounter.Increment; // TODO: check this down casting..
                    break;
                // TODO: others
            }
        };

        public Action<CrdtState> ApplyState => (s) => {
            switch (s.StateCase)
            {
                case CrdtState.StateOneofCase.Gcounter:
                    Value = (long)s.Gcounter.Value; // TODO: Check casting
                    break;
            }
        };
        public long Increment(long by)
        {
            if (by < 0) {
                throw new ArgumentOutOfRangeException(nameof(by), "Cannot increment a GCounter by a negative amount.");
            }
            DeltaValue += by;
            Value += by; 
            return Value;
        }
        
        public long GetValue => Value;
        
    }
    
}