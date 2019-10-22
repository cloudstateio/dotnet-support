using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Cloudstate;
using Google.Protobuf;
using Google.Protobuf.Reflection;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using io.cloudstate.csharpsupport.eventsourced.impl;
using io.cloudstate.csharpsupport.impl;
using Microsoft.Extensions.Logging;

namespace io.cloudstate.csharpsupport.impl
{
    /// <summary>
    /// Entity Discovery Service
    /// </summary>
    public class EntityDiscoveryService : EntityDiscovery.EntityDiscoveryBase
    {

        private ILoggerFactory LoggerFactory { get; }
        private IDictionary<string, IStatefulService> Services { get; }

        ILogger<EntityDiscoveryService> Logger { get; }
        ServiceInfo ServiceInfo { get; }

        /// <summary>
        /// Creates a gRPC service for enabling CloudState entity discovery
        /// </summary>
        /// <param name="loggerFactory"></param>
        /// <param name="services"></param>
        public EntityDiscoveryService(ILoggerFactory loggerFactory, IDictionary<string, IStatefulService> services)
        {
            LoggerFactory = loggerFactory;
            Services = services;

            Logger = LoggerFactory.CreateLogger<EntityDiscoveryService>();
            var runtimeName = Environment.GetEnvironmentVariable("DOTNET_RUNTIME_NAME") ?? "";
            var runtimeVersion = Environment.GetEnvironmentVariable("DOTNET_RUNTIME_VERSION") ?? "";
            ServiceInfo = new ServiceInfo()
            {
                ServiceRuntime = runtimeName + " " + runtimeVersion,
                SupportLibraryName = "cloudstate-csharp-support",
                SupportLibraryVersion = Assembly.GetEntryAssembly()?.GetName()?.Version?.ToString() ?? ""
            };
        }

        /// <summary>
        /// Service discovery endpoint called by the CloudState operator in order to determine
        /// the list of <see cref="IStatefulService" />'s running within the current local process.
        /// </summary>
        /// <param name="request">ProxyInfo request</param>
        /// <param name="context">ServerCallContext</param>
        /// <returns></returns>
        public override async Task<Cloudstate.EntitySpec> discover(Cloudstate.ProxyInfo request, ServerCallContext context)
        {
            Logger.LogInformation(
                $"Received discovery call from sidecar [{request.ProxyName} {request.ProxyVersion}] supporting CloudState {request.ProtocolMajorVersion}.{request.ProtocolMinorVersion}"
            );
            Logger.LogDebug($"Supported sidecar entity types: {String.Join(", ", request.SupportedEntityTypes)}");

            var unsupportedServices = Services.Values.Where(service =>
                !request.SupportedEntityTypes.Contains(service.EntityType)
            );

            if (unsupportedServices.Any())
            {
                Logger.LogError(
                    "Proxy doesn't support the entity types for the following services: " +
                        String.Join(", ", unsupportedServices
                        .Select(s => s.Descriptor.FullName + ": " + s.EntityType)
                    )
                );
            }

            if (false)
            {
                // TODO: verify compatibility with in.protocolMajorVersion & in.protocolMinorVersion 
                // await Task.FromException(new Exception("Proxy version not compatible with library protocol support version"));
            }
            else
            {

                var allDescriptors = new AnySupport(Services.Values.Select(x => x.Descriptor.File)).FlattenDescriptors(
                    Services.Values.Select(x => x.Descriptor.File)
                );

                var set = new FileDescriptorSet();
                set.File.AddRange(allDescriptors.Select(x => x.Value.Proto));
                var fileDescriptorSet = set.ToByteString();
                var entities = Services.Select(x =>
                    new Entity()
                    {
                        EntityType = x.Value.EntityType,
                        ServiceName = x.Key,
                        PersistenceId = x.Value.PersistenceId
                    }
                );

                var spec = new EntitySpec()
                {
                    ServiceInfo = ServiceInfo,
                    Proto = fileDescriptorSet
                };

                try
                {
                    spec.Entities.AddRange(entities);
                }
                catch (Exception ex)
                {
                    Logger.LogError($"Exception: {ex.StackTrace}", ex.InnerException ?? ex);
                }

                return await Task.FromResult(spec);
            }

        }

        /// <summary>
        /// Handle an error raised from the sidecar with reference to the user-funciton.
        /// </summary>
        /// <param name="request">UserFunctionError instance</param>
        /// <param name="context">ServerCallContext</param>
        /// <returns></returns>
        public override async Task<Empty> reportError(Cloudstate.UserFunctionError request, ServerCallContext context)
        {
            Logger.LogError($"Error reported from sidecar: {request.Message}");
            return await Task.FromResult(new Empty());
        }

    }

}