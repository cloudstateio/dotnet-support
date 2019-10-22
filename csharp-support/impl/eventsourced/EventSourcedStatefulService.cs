using System;
using System.Collections.Generic;
using Cloudstate.Eventsourced;
using Google.Protobuf.Reflection;
using io.cloudstate.csharpsupport.eventsourced;
using io.cloudstate.csharpsupport.eventsourced.impl;

namespace io.cloudstate.csharpsupport.impl.eventsourced
{
    public sealed class EventSourcedStatefulService : IStatefulService
    {
        public IEventSourcedEntityFactory Factory { get; }
        public AnySupport AnySupport { get; }
        public ServiceDescriptor Descriptor { get; }
        public int SnapshotEvery { get; }
        public string PersistenceId { get; }

        public String EntityType => EventSourced.Descriptor.FullName;

        public Dictionary<string, IResolvedServiceMethod> ResolvedMethods { get; }

        public EventSourcedStatefulService(
            IEventSourcedEntityFactory factory,
            ServiceDescriptor descriptor,
            AnySupport anySupport,
            string persistenceId,
            int snapshotEvery
        )
        {

            Factory = factory;
            Descriptor = descriptor;
            AnySupport = anySupport;
            SnapshotEvery = snapshotEvery;
            PersistenceId = persistenceId;

            switch (Factory)
            {
                case IResolvedEntityFactory resolved:
                    ResolvedMethods = resolved.ResolvedMethods;
                    break;
                default:
                    ResolvedMethods = new Dictionary<string, IResolvedServiceMethod>();
                    break;
            }


        }

        public EventSourcedStatefulService WithSnapshotEvery(int snapshotEvery)
        {
            if (SnapshotEvery == snapshotEvery)
                return this;
            return new EventSourcedStatefulService(
                Factory,
                Descriptor,
                AnySupport,
                PersistenceId,
                SnapshotEvery
            );
        }

    }

}
