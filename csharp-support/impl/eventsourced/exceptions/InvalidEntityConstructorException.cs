using System;

namespace io.cloudstate.csharpsupport.impl.eventsourced
{
    public class InvalidEntityConstructorException : Exception
    {
        public InvalidEntityConstructorException(string message) : base(message) { }
    }


}