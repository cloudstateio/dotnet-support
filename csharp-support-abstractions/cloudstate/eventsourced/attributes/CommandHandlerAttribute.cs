using System;

namespace io.cloudstate.csharpsupport.eventsourced
{
    /// <summary>
    /// Cloud state event sourced entity command handler
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class CommandHandlerAttribute : CloudStateAttribute
    {
        /// <summary>
        /// The name of the command to handle.
        /// If not specified, the name of the method will be used as the command name, with the first
        /// letter capitalized to match the gRPC convention of capitalizing rpc method names.
        /// </summary>
        /// <value></value>
        public string Name { get; } = "";
    }
}
