using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using CloudState.CSharpSupport.Crdt.Interfaces;
using CloudState.CSharpSupport.EventSourced.Abstract;
using CloudState.CSharpSupport.Interfaces;
using CloudState.CSharpSupport.Interfaces.Crdt;
using CloudState.CSharpSupport.Interfaces.Crdt.Contexts;
using CloudState.CSharpSupport.Reflection.Interfaces;
using CloudState.CSharpSupport.Serialization;
using Cloudstate.Function;
using Google.Protobuf.Reflection;
using Optional;

namespace CloudState.CSharpSupport
{
    internal sealed class CrdtStatefulService : StatefulEntityService<ICrdtEntityCreationContext, ICrdtEntityHandler>, ICrdtStatefulService
    {
        private string[] Streamed { get; }
        public override string StatefulServiceTypeName => Cloudstate.Crdt.Crdt.Descriptor.Name;
        public ICrdtEntityHandlerFactory Factory { get; }

        public CrdtStatefulService(
            ICrdtEntityHandlerFactory factory,
            ServiceDescriptor descriptor,
            AnySupport anySupport) : base(factory, descriptor, anySupport)
        {
            Factory = factory;
            Streamed = descriptor.Methods.Where(x => x.IsServerStreaming).Select(x => x.Name).ToArray();
        }
        public bool IsStreamed(string command) => Streamed.Contains(command);

    }
}