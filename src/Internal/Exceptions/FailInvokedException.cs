using System;
using System.Runtime.Serialization;

namespace CloudState.CSharpSupport.Exceptions
{
    [Serializable]
    public class FailInvokedException : Exception
    {
        public FailInvokedException() : base("CommandContext.Fail(...) invoked")
        {

        }

        protected FailInvokedException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {

        }
    }


}