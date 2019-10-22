using System;

namespace io.cloudstate.csharpsupport.eventsourced
{
    /// <summary>
    /// Cloud state event sourced entity
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class EventSourcedEntityAttribute : CloudStateAttribute
    {

        public String PersistenceId { get; set; } = "";

        public int SnapshotEvery { get; set; } = 0;

    }

}