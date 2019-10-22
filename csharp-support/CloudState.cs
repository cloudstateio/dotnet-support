using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;
using io.cloudstate.csharpsupport.eventsourced.impl;
using io.cloudstate.csharpsupport.impl.eventsourced;
using io.cloudstate.csharpsupport.eventsourced;
using Microsoft.Extensions.DependencyInjection;
using ServiceDescriptor = Google.Protobuf.Reflection.ServiceDescriptor;
using System.Threading;
using io.cloudstate.csharpsupport.impl;
using Google.Protobuf.Reflection;

namespace io.cloudstate.csharpsupport
{
    public class CloudState
    {

        string TypeUrlPrefix { get; set; } = AnySupport.DefaultTypeUrlPrefix;

        Dictionary<String, IStatefulService> StatefulServices { get; }
            = new Dictionary<string, IStatefulService>();

        public CloudState WithTypeUrlPrefix(string typeUrlPrefix)
        {
            TypeUrlPrefix = typeUrlPrefix;
            return this;
        }

        public CloudState RegisterEventSourcedEntity(
            IEventSourcedEntityFactory factory,
            ServiceDescriptor descriptor,
            String persistenceId,
            int snapshotEvery,
            params FileDescriptor[] additionalDescriptors
        )
        {
            StatefulServices.Add(
                descriptor.FullName,
                new EventSourcedStatefulService(
                    factory,
                    descriptor,
                    NewAnySupport(additionalDescriptors),
                    persistenceId,
                    snapshotEvery
                )
            );

            return this;
        }

        /// <summary>
        /// Register an event sourced entity to be used when starting the cloudstate worker.
        /// </summary>
        /// <param name="descriptor">gRPC service descriptor for the given entity</param>
        /// <param name="additionalDescriptors">Additional protobuf file descriptors to support
        /// gRPC calls and event store persistence</param>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <returns>Cloudstate</returns>
        public CloudState RegisterEventSourcedEntity<TEntity>(
            ServiceDescriptor descriptor,
            params Google.Protobuf.Reflection.FileDescriptor[] additionalDescriptors
        )
        {

            var entity = typeof(TEntity).GetCustomAttributes(
                    typeof(EventSourcedEntityAttribute), true
                ).FirstOrDefault() as EventSourcedEntityAttribute;

            if (entity == null)
            {
                throw new InvalidOperationException(
                    $"{typeof(TEntity)} does not declare a(n) {typeof(EventSourcedEntityAttribute).Name} attribute!");
            }

            String persistenceId;
            int snapshotEvery;

            if (String.IsNullOrEmpty(entity.PersistenceId))
            {
                persistenceId = typeof(TEntity).Name;
                snapshotEvery = 0;
            }
            else
            {
                persistenceId = entity.PersistenceId;
                snapshotEvery = entity.SnapshotEvery;
            }

            var anySupport = NewAnySupport(additionalDescriptors);
            StatefulServices.Add(
                descriptor.FullName,
                new EventSourcedStatefulService(
                    new AnnotationBasedEventSourcedSupport<TEntity>(anySupport, descriptor),
                    descriptor,
                    anySupport,
                    persistenceId,
                    snapshotEvery
                )
            );

            return this;

        }

        public async Task StartAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            await Host.CreateDefaultBuilder()
                .ConfigureServices((_, services) =>
                {
                    services.AddSingleton<IDictionary<String, IStatefulService>>(
                        StatefulServices.ToDictionary(
                            x => x.Key, x => x.Value
                        )
                    );
                    services.AddHostedService<CloudStateWorker>();
                })
                .RunConsoleAsync(cancellationToken);
        }

        private AnySupport NewAnySupport(FileDescriptor[] additionalDescriptors)
        {
            return new AnySupport(additionalDescriptors, TypeUrlPrefix);
        }

    }

}