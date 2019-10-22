using System;
using System.Runtime.Serialization;

namespace io.cloudstate.csharpsupport {

    [Serializable]
    public class CloudStateException : Exception
    {
        public CloudStateException(string message) : base(message)
        {

        }

        protected CloudStateException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {

        }
    }

}