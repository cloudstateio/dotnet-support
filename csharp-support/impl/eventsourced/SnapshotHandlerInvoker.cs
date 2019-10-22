using System.Reflection;
using Google.Protobuf.WellKnownTypes;

namespace io.cloudstate.csharpsupport.impl.eventsourced
{
    public class SnapshotHandlerInvoker
    {

        public MethodInfo Method { get; }

        public SnapshotHandlerInvoker(MethodInfo method)
        {
            Method = method;
        }
    }

}
