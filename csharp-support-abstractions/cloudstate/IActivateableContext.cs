using System;
using System.Collections.Generic;
using Cloudstate;

namespace io.cloudstate.csharpsupport
{
    public interface IActivateableContext : IContext
    {
        bool Inactive { get; protected set; }
        void Deactivate()
        {
            Inactive = true;
        }
        void CheckActive()
        {
            if (Inactive)
                throw new InvalidOperationException("Context no longer active");
        }
    }
}