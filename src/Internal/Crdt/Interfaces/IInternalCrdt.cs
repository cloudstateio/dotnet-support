using System;
using Cloudstate.Crdt;
using CloudState.CSharpSupport.Interfaces.Crdt.Elements;
using Optional;

namespace CloudState.CSharpSupport.Crdt.Interfaces
{
    internal interface IInternalCrdt : ICrdt
    {
        string Name { get; }
        bool HasDelta { get; }
        Option<CrdtDelta> Delta { get; }
        void ResetDelta();
        CrdtState State { get; }
        Action<CrdtDelta> ApplyDelta { get; }
        Action<CrdtState> ApplyState { get; }
    }
}