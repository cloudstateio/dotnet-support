using System;

namespace io.cloudstate.csharpsupport
{
    public interface IEntityContext : IContext
    {
        String EntityId { get; }
    }
}