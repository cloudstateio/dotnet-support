using System;
using System.Runtime.Serialization;

namespace io.cloudstate.csharpsupport.impl.eventsourced
{
    [Serializable]
    public class InvalidEntityConstructorException : Exception
    {
        public InvalidEntityConstructorException(string message) : base(message) { }

        protected InvalidEntityConstructorException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {

        }
    }


}