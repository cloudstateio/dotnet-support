using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using CloudState.CSharpSupport.Attributes.EventSourced;
using CloudState.CSharpSupport.EventSourced;
using CloudState.CSharpSupport.Interfaces;
using CloudState.CSharpSupport.Interfaces.EventSourced;
using CloudState.CSharpSupport.Serialization;
using Google.Protobuf.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ServiceDescriptor = Google.Protobuf.Reflection.ServiceDescriptor;

namespace CloudState.CSharpSupport
{
    public class CloudState
    {
        private string TypeUrlPrefix { get; set; }
        private Dictionary<string, IStatefulService> StatefulServices { get; }
            = new Dictionary<string, IStatefulService>();

        public CloudState WithTypeUrlPrefix(string typeUrlPrefix)
        {
            TypeUrlPrefix = typeUrlPrefix;
            return this;
        }

        public CloudState RegisterEventSourcedEntity(
            IEventSourcedEntityHandlerFactory factory,
            ServiceDescriptor descriptor,
            string persistenceId,
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
        /// Register an event sourced entity to be used when starting the CloudState worker.
        /// </summary>
        /// <param name="descriptor">gRPC service descriptor for the given entity</param>
        /// <param name="additionalDescriptors">Additional Protobuf file descriptors to support
        /// gRPC calls and event store persistence</param>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <returns>CloudState</returns>
        public CloudState RegisterEventSourcedEntity<TEntity>(
            ServiceDescriptor descriptor,
            params FileDescriptor[] additionalDescriptors
        )
        {

            var entity = typeof(TEntity).GetCustomAttribute<EventSourcedEntityAttribute>(true);
            if (entity == null)
                throw new InvalidOperationException(
                    $"{typeof(TEntity)} does not declare a(n) {typeof(EventSourcedEntityAttribute).Name} attribute!"
                );

            string persistenceId;
            int snapshotEvery;

            if (string.IsNullOrEmpty(entity.PersistenceId))
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
                    new AttributeBasedEntityHandlerFactory(
                        typeof(TEntity),
                        anySupport,
                        descriptor
                    ),
                    descriptor,
                    anySupport,
                    persistenceId,
                    snapshotEvery
                )
            );

            return this;

        }

        public async Task StartAsync(CancellationToken cancellationToken = default)
        {
            await Host.CreateDefaultBuilder()
                .ConfigureServices((_, services) =>
                {
                    services.AddSingleton<IDictionary<string, IStatefulService>>(
                        StatefulServices.ToDictionary(
                            x => x.Key, x => x.Value
                        )
                    );
                    services.AddHostedService<CloudStateWorker>();
                })
                .RunConsoleAsync(cancellationToken);
        }

        private AnySupport NewAnySupport(params FileDescriptor[] additionalDescriptors)
        {
            return new AnySupport(additionalDescriptors, TypeUrlPrefix);
        }

    }

}