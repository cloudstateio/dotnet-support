using System;

namespace io.cloudstate.csharpsupport.eventsourced
{
    /// <summary>
    /// Cloud state event sourced entity snapshot handler
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class SnapshotHandlerAttribute : CloudStateAttribute { }

}