using System;

namespace io.cloudstate.csharpsupport.eventsourced
{
    /// <summary>
    /// Cloud state event sourced entity event handler
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class EventHandlerAttribute : CloudStateAttribute
    {
        public Type EventClass { get; }
        public EventHandlerAttribute(Type? eventClass = null)
        {
            EventClass = eventClass ?? typeof(Object);
        }
    }
}