using System;

namespace io.cloudstate.csharpsupport.eventsourced
{
    [AttributeUsage(AttributeTargets.Parameter)]
    public class EntityIdAttribute : CloudStateAttribute { }
}