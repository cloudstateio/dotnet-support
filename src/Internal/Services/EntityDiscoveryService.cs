using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Cloudstate;
using CloudState.CSharpSupport.Interfaces;
using CloudState.CSharpSupport.Serialization;
using Google.Protobuf;
using Google.Protobuf.Reflection;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.Extensions.Logging;

namespace CloudState.CSharpSupport.Services
{
    /// <summary>
    /// Service provided by gRPC for the discovery of entity based gRPC services
    /// running on the local server 
    /// </summary>
    internal class EntityDiscoveryService : EntityDiscovery.EntityDiscoveryBase
    {

        private ILoggerFactory LoggerFactory { get; }
        private IReadOnlyDictionary<string, IStatefulService> Services { get; }

        private ILogger<EntityDiscoveryService> Logger { get; }
        private ServiceInfo ServiceInfo { get; }

        /// <summary>
        /// Creates a gRPC service for enabling CloudState entity discovery
        /// </summary>
        /// <param name="loggerFactory"></param>
        /// <param name="services"></param>
        public EntityDiscoveryService(ILoggerFactory loggerFactory, IReadOnlyDictionary<string, IStatefulService> services)
        {
            LoggerFactory = loggerFactory;
            Services = services;

            Logger = LoggerFactory.CreateLogger<EntityDiscoveryService>();
            var runtimeName = Environment.GetEnvironmentVariable("DOTNET_RUNTIME_NAME") ?? "";
            var runtimeVersion = Environment.GetEnvironmentVariable("DOTNET_RUNTIME_VERSION") ?? "";
            ServiceInfo = new ServiceInfo
            {
                ServiceRuntime = runtimeName + " " + runtimeVersion,
                SupportLibraryName = "CloudState.CSharpSupport",
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
        public override async Task<EntitySpec> discover(ProxyInfo request, ServerCallContext context)
        {
            Logger.LogInformation(
                $"Received discovery call from sidecar [{request.ProxyName} {request.ProxyVersion}] supporting CloudState {request.ProtocolMajorVersion}.{request.ProtocolMinorVersion}"
            );
            Logger.LogDebug($"Supported sidecar entity types: {string.Join(", ", request.SupportedEntityTypes)}");

            var unsupportedServices = Services.Values.Where(service =>
                !request.SupportedEntityTypes.Contains(service.StatefulServiceTypeName)
            );

            if (unsupportedServices.Any())
            {
                Logger.LogError(
                    "Proxy doesn't support the entity types for the following services: " +
                        string.Join(", ", unsupportedServices
                        .Select(s => s.ServiceDescriptor.FullName + ": " + s.StatefulServiceTypeName)
                    )
                );
            }

            if (false)
            {
                // TODO: verify compatibility with in.protocolMajorVersion & in.protocolMinorVersion 
                // await Task.FromException(new CloudStateException("Proxy version not compatible with library protocol support version"));
            }
            else
            {

                var allDescriptors = new AnySupport(Services.Values.Select(x => x.ServiceDescriptor.File)).FlattenDescriptors(
                    Services.Values.Select(x => x.ServiceDescriptor.File)
                );

                var set = new FileDescriptorSet();
                set.File.AddRange(allDescriptors.Select(x => FileDescriptorProto.Parser.ParseFrom(x.Value.SerializedData)));
                var fileDescriptorSet = set.ToByteString();
                var entities = Services.Select(x =>
                    new Entity
                    {
                        EntityType = x.Value.StatefulServiceTypeName,
                        ServiceName = x.Key,
                        PersistenceId = x.Value.PersistenceId
                    }
                );

                var spec = new EntitySpec
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
        public override async Task<Empty> reportError(UserFunctionError request, ServerCallContext context)
        {
            Logger.LogError($"Error reported from sidecar: {request.Message}");
            return await Task.FromResult(new Empty());
        }

    }

}