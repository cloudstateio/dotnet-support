using System;
using Microsoft.Extensions.Configuration;

namespace io.cloudstate.csharpsupport
{
    /// <summary>
    /// Cloudstate configuration
    /// </summary>
    public class Configuration
    {
        public string Host { get; }
        public int Port { get; }
        public int SnapshotEvery { get; }

        /// <summary>
        /// Construct a cloud state worker configuration from an Akka 
        /// configuration entity
        /// </summary>
        /// <param name="config">.NET configuration entity</param>
        public Configuration(IConfiguration config)
        {
            Host = config.GetValue<string>("user-function-interface") ?? "0.0.0.0";
            Port = config.GetValue<int?>("user-function-port") ?? 8080;
            SnapshotEvery = config.GetValue<int?>("eventsourced.snapshot-every") ?? 100;
        }
    }
}