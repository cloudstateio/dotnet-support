using System;

namespace io.cloudstate.csharpsupport.eventsourced
{
    /// <summary>
    /// Cloud state event sourced snapshot
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class SnapshotAttribute : CloudStateAttribute { }

}