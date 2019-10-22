using System;

namespace io.cloudstate.csharpsupport.eventsourced
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface)]
    public class JsonableAttribute : CloudStateAttribute { }
}