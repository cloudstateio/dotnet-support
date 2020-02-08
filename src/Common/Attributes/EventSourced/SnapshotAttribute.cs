using System;

namespace CloudState.CSharpSupport.Attributes.EventSourced
{
    /// <summary>
    /// Cloud state event sourced snapshot
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class SnapshotAttribute : CloudStateAttribute { }

}