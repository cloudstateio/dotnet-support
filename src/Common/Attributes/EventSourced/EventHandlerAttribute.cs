using System;
using CloudState.CSharpSupport.Interfaces.EventSourced.Contexts;

namespace CloudState.CSharpSupport.Attributes.EventSourced
{
    /// <summary>
    /// Cloud state event sourced entity event handler.
    /// 
    /// For classes annotated with <see cref="EventSourcedEntityAttribute"/>,
    /// it is used to signify methods which act as event handlers.  The
    /// event handling method may take an <see cref="IEventContext"/> in
    /// addition to the main argument meant to be handled.  It may also
    /// receive a parameter annotated with <see cref="EntityIdAttribute"/>
    /// which must be of type <see cref="System.String"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class EventHandlerAttribute : CloudStateAttribute
    {
        /// <summary>
        /// The type of class belonging to the event.
        /// </summary>
        public Type EventClass { get; }

        /// <summary>
        /// Event handler attribute
        /// </summary>
        /// <param name="eventClass">Event type.  If not specified, defaults to <see cref="Object"/>.</param>
        public EventHandlerAttribute(Type eventClass = null)
        {
            EventClass = eventClass ?? typeof(object);
        }
    }
}