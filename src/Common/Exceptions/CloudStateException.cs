using System;

namespace CloudState.CSharpSupport.Exceptions
{
    /// <summary>
    /// Base class for all CloudState exceptions
    /// </summary>
    public class CloudStateException : Exception
    {
        /// <summary>
        /// Creates a new instance of a CloudState exception
        /// </summary>
        /// <param name="message">Descriptive message</param>
        public CloudStateException(string message) : base(message) { }
    }
}
