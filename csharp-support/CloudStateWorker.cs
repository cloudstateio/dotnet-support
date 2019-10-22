using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudstate;
using Cloudstate.Eventsourced;
using Grpc.Core;
using io.cloudstate.csharpsupport.eventsourced.impl;
using io.cloudstate.csharpsupport.impl;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace io.cloudstate.csharpsupport
{
    internal partial class CloudStateWorker : IHostedService
    {
        ILoggerFactory LoggerFactory { get; }
        IDictionary<String, IStatefulService> StatefulServices { get; }

        Configuration Config { get; }
        ILogger<CloudStateWorker> Logger { get; }
        Server Server { get; }

        public CloudStateWorker(ILoggerFactory loggerFactory, IConfiguration configuration, IDictionary<String, IStatefulService> statefulServices)
        {
            LoggerFactory = loggerFactory;
            StatefulServices = statefulServices;

            Config = new Configuration(configuration);
            Logger = LoggerFactory.CreateLogger<CloudStateWorker>();
            Server = new Server()
            {
                Ports = { new ServerPort(Config.Host, Config.Port, ServerCredentials.Insecure) }
            };
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await Task.Factory.StartNew(() =>
            {

                foreach (var serviceGroup in StatefulServices.GroupBy(x => x.Value.EntityType))
                {
                    switch (serviceGroup.Key)
                    {
                        case "cloudstate.eventsourced.EventSourced":
                            Server.Services.Add(
                                EventSourced.BindService(new EventSourcedService(
                                    LoggerFactory,
                                    Config,
                                    serviceGroup.ToDictionary(
                                        x => x.Key, x => x.Value
                                    ),
                                    new Context(new ResolvedServiceCallFactory(StatefulServices))
                                ))
                            );
                            break;
                        default:
                            throw new NotImplementedException($"Unknown stateful service implementation of {serviceGroup.Key}");
                    }
                }

                Server.Services.Add(
                    EntityDiscovery.BindService(
                        new EntityDiscoveryService(
                            LoggerFactory,
                            StatefulServices
                        )
                    )
                );

                Server.Start();
                Logger.LogInformation(
                    $"Server listening on [{Config.Host}:{Config.Port}]"
                );

            });
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await Server.ShutdownAsync();
        }
    }
}