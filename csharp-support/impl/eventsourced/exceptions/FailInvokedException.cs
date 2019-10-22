using System;

namespace io.cloudstate.csharpsupport.impl.eventsourced
{
    public class FailInvokedException : Exception
    {
        public FailInvokedException() : base("CommandContext.Fail(...) invoked")
        {

        }
    }


}