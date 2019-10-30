using System;

namespace CloudState.CSharpSupport.Exceptions
{
    public class CloudStateException : Exception
    {
        public CloudStateException(string message) : base(message) { }
    }
}
