using System;
using CloudState.CSharpSupport.Contexts.Interfaces;

namespace CloudState.CSharpSupport.Contexts.Abstractions
{
    internal class AbstractActivatableContext : IActivatableContext
    {
        public bool Active { get; private set; } = true;

        public void Deactivate() => Active = false;
        public void CheckActive()
        {
            if (!Active) throw new InvalidOperationException("Context no longer active!");
        }
    }
}
