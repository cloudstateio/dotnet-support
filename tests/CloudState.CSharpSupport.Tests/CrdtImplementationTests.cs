// using System.Collections.Generic;
// using System.Linq;
// using CloudState.CSharpSupport.Contexts;
// using CloudState.CSharpSupport.Crdt;
// using CloudState.CSharpSupport.Crdt.Interfaces;
// using CloudState.CSharpSupport.Crdt.Services;
// using CloudState.CSharpSupport.Interfaces;
// using CloudState.CSharpSupport.Reflection;
// using CloudState.CSharpSupport.Serialization;
// using Com.Example.Crdts;
// using Com.Example.Shoppingcart;
// using Crdt.Example;
// using EventSourced.ShoppingCart;
// using Microsoft.Extensions.Logging;
// using Xunit;

// namespace CloudState.CSharpSupport.Tests
// {
//     public class CrdtImplementationTests
//     {
//         static CrdtEntityCollectionService SetupCrdtImpl<T>(
//             Google.Protobuf.Reflection.ServiceDescriptor descriptor,
//             params Google.Protobuf.Reflection.FileDescriptor[] additionalDescriptors
//         )
//         {
//             var anySupport = new AnySupport(additionalDescriptors);
//             IReadOnlyDictionary<string, ICrdtStatefulService> services = new Dictionary<string, ICrdtStatefulService>
//             {
//                 {
//                     descriptor.FullName, new CrdtStatefulService(
//                         new AttributeBasedCrdtHandlerFactory(
//                             typeof(CrdtExampleEntity), anySupport, descriptor),
//                         descriptor,
//                         anySupport
//                     )
//                 }
//             };

//             var rootContext =
//                 new Context(
//                     new ResolvedServiceCallFactory(services.ToDictionary(x => x.Key,
//                         x => x.Value as IStatefulService)));

//             var impl = new CrdtEntityCollectionService(
//                 LoggerFactory.Create(x => x.AddConsole()),
//                 services,
//                 rootContext
//             );

//             return impl;

//         }

//         [Fact]
//         public void CanInstantiate()
//         {
//             var impl = SetupCrdtImpl<CrdtExampleEntity>(
//                 CrdtExample.Descriptor,
//                 CrdtExampleReflection.Descriptor
//             );

//             Assert.NotNull(impl);
//         }
//     }
// }