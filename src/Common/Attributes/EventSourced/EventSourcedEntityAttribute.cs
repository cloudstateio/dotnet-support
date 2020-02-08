using System;

namespace CloudState.CSharpSupport.Attributes.EventSourced
{
    /// <summary>
    /// Cloud state event sourced entity
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class EventSourcedEntityAttribute : CloudStateAttribute
    {

        /// <summary>
        /// PersistenceId for the target entity
        /// </summary>
        public string PersistenceId { get; }

        /// <summary>
        /// Snapshot interval for triggering every n-th event
        /// </summary>
        public int SnapshotEvery { get; }

        /// <summary>
        /// Annotation to signify a the usage of a class as a CloudState entity
        /// </summary>
        /// <param name="persistenceId"></param>
        /// <param name="snapshotEvery"></param>
        public EventSourcedEntityAttribute(string persistenceId = "", int snapshotEvery = 0)
        {
            PersistenceId = persistenceId;
            SnapshotEvery = snapshotEvery;
        }

    }

}