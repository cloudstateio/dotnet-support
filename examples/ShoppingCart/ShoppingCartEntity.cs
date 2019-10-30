using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using CloudState.CSharpSupport.Attributes;
using CloudState.CSharpSupport.Attributes.EventSourced;
using Com.Example.Shoppingcart;

namespace csharp_support
{
    [EventSourcedEntity]
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public class ShoppingCartEntity
    {
        private string EntityId { get; }
        private Dictionary<string, LineItem> Cart { get; }

        public ShoppingCartEntity([EntityId]string entityId)
        {
            EntityId = entityId;
            Cart = new Dictionary<string, LineItem>();
        }

        [CommandHandler]
        public Cart GetCart()
        {
            var cart = new Cart();
            cart.Items.AddRange(Cart.Values);
            return cart;
        }
    }
}
