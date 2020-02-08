// using System.Collections.Generic;
// using CloudState.CSharpSupport.Reflection.ReflectionHelper;
// using CloudState.CSharpSupport.Tests.AttributeBasedEntityFactory;
// using CloudState.CSharpSupport.Tests.AttributeBasedEntityFactory.CommandHandler;
// using CloudState.CSharpSupport.Tests.AttributeBasedEntityFactory.CommandHandler.ResolvedTypes;
// using Google.Protobuf.WellKnownTypes;
// using Moq;
// using Optional;
// using Xunit;

// namespace CloudState.CSharpSupport.Tests.AttributeBasedCrdtEntityFactory
// {
//     public class EntityConstructionTests
//     {
//         [Fact]
//         public void ShouldSupportNoArgConstructorTest()
//         {
//             Create<NoArgConstructorEntity>();
//         }

//         private ICrdtEntityHandler Create<T>()
//             where T : new()
//         {
//             return new CrdtEntityHandler(
//                 typeof(T),
//                 new T(),
//                 new Dictionary<string, ReflectionHelper.CommandHandlerInvoker<ICommandContext>>(),
//                 new Dictionary<string, ReflectionHelper.CommandHandlerInvoker<ICommandContext>>());
//         }
//     }
// }