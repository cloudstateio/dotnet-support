using System.Diagnostics.CodeAnalysis;
using CloudState.CSharpSupport.Attributes.EventSourced;
using CloudState.CSharpSupport.Interfaces.EventSourced.Contexts;
using CloudState.CSharpSupport.Tests.AttributeBasedEntityFactory.CommandHandler.Messages;
using Com.Example.Shoppingcart;
using Com.Example.Shoppingcart.Persistence;
using Google.Protobuf.WellKnownTypes;
using Xunit;

namespace CloudState.CSharpSupport.Tests.AttributeBasedEntityFactory.CommandHandler.Entities
{
    [EventSourcedEntity]
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
    public class AllowEmittingEventsEntity
    {
        [CommandHandler]
        public Wrapped AddItem(string msg, ICommandContext context)
        {
            Assert.Equal("AddItem", context.CommandName);
            context.Emit(msg + " event");
            return new Wrapped(msg);
        }
    }
}