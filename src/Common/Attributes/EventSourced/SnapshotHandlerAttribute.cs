using System;

namespace CloudState.CSharpSupport.Attributes.EventSourced
{
    /// <summary>
    /// Cloud state event sourced entity snapshot handler
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class SnapshotHandlerAttribute : CloudStateAttribute { }

}