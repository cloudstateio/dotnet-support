using System;
using CloudState.CSharpSupport.Interfaces.EventSourced.Contexts;

namespace CloudState.CSharpSupport.Attributes.EventSourced
{
    /// <summary>
    /// Cloud state event sourced entity command handler
    ///
    /// For classes annotated with <see cref="EventSourcedEntityAttribute"/>,
    /// it is used to signify methods which act as command handlers.  The
    /// command handling method may take an <see cref="ICommandContext"/> in
    /// addition to the main argument meant to be handled.  It may also
    /// receive a parameter annotated with <see cref="EntityIdAttribute"/>
    /// which must be of type <see cref="System.String"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
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
