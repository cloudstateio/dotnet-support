// using System;
// using System.Linq;
// using CloudState.CSharpSupport.Crdt;
// using CloudState.CSharpSupport.Crdt.Elements;
// using CloudState.CSharpSupport.Interfaces.Crdt;
// using CloudState.CSharpSupport.Interfaces.Crdt.Contexts;
// using CloudState.CSharpSupport.Interfaces.Crdt.Elements;
// using CloudState.CSharpSupport.Serialization;
// using Com.Example.Crdts;
// using Crdt.Example;
// using Google.Protobuf.WellKnownTypes;
// using Moq;
// using Optional;
// using Xunit;
// using Type = System.Type;

// namespace CloudState.CSharpSupport.Tests.AttributeBasedCrdtEntityFactory.CommandHandler
// {
//     public class AttributeBasedCrdtEntityFactoryCommandHandlerTests
//     {
//         [Fact]
//         public void CanHandleCommand()
//         {
//             var mockContext = new Mock<ICommandContext>();
//             mockContext.Setup(x => x.CommandName).Returns("IncrementGCounter");
//             mockContext.Setup(x => x.State<IGCounter>(It.IsAny<Type>())).Returns((Type x) => new GCounterImpl().Some<IGCounter>());
//             var handler = Helpers.CreateHandler<CrdtExampleEntity>(x => new CrdtExampleEntity());
//             var command = new UpdateCounter {Key = "ok", Value = 100 };
//             var msg = Any.Pack(command);
//             var result = handler.HandleCommand(msg, mockContext.Object);
//             // TODO: Assert
//         }
//     }

//     public class Helpers
//     {
//         public static ICrdtEntityHandler CreateHandler<T>(Func<ICrdtEntityCreationContext, object> entityFactory = null)
//         {
//             var anySupport = new AnySupport(
//                 new[] { CrdtExampleReflection.Descriptor }
//             );
//             var mockSupport = new Mock<ICrdtEntityCreationContext>();
//             mockSupport.Setup(x => x.EntityId)
//                 .Returns("foo");

//             var methods = anySupport.ResolveServiceDescriptor(CrdtExample.Descriptor)
//                 .Where(x => x.Key == "IncrementGCounter")
//                 .ToDictionary(x => x.Key, x => x.Value);

//             return new AttributeBasedCrdtHandlerFactory(
//                 typeof(T),
//                 anySupport,
//                 entityFactory,
//                 methods
//             ).CreateEntityHandler(mockSupport.Object);
//         }
//     }
// }