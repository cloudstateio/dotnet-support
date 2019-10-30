using System;

namespace CloudState.CSharpSupport.Attributes.EventSourced
{
    /// <summary>
    /// Cloud state event sourced entity
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class EventSourcedEntityAttribute : CloudStateAttribute
    {

        public string PersistenceId { get; set; } = "";

        public int SnapshotEvery { get; set; } = 0;

    }

}