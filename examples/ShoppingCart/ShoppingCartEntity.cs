using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using CloudState.CSharpSupport.Attributes;
using CloudState.CSharpSupport.Attributes.EventSourced;
using CloudState.CSharpSupport.Interfaces.EventSourced.Contexts;
using Com.Example.Shoppingcart.Persistence;
using Google.Protobuf.WellKnownTypes;

namespace EventSourced.ShoppingCart
{
    [EventSourcedEntity]
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public class ShoppingCartEntity
    {
        String EntityId { get; }

        Dictionary<String, Com.Example.Shoppingcart.LineItem> Cart { get; }

        public ShoppingCartEntity([EntityId]String entityId)
        {
            EntityId = entityId;
            Cart = new Dictionary<string, Com.Example.Shoppingcart.LineItem>();
        }

        [Snapshot]
        public Com.Example.Shoppingcart.Persistence.Cart Snapshot()
        {
            var cart = new Com.Example.Shoppingcart.Persistence.Cart();
            cart.Items.AddRange(Cart.Select(x => Convert(x.Value)));
            return cart;
        }

        [SnapshotHandler]
        public void HandleSnapshot(Com.Example.Shoppingcart.Persistence.Cart cart)
        {
            Cart.Clear();
            foreach (Com.Example.Shoppingcart.Persistence.LineItem item in cart.Items)
            {
                Cart.Add(item.ProductId, Convert(item));
            }
        }

        [EventHandler(typeof(ItemAdded))]
        public void ItemAdded(Com.Example.Shoppingcart.Persistence.ItemAdded itemAdded)
        {
            Cart.TryGetValue(itemAdded.Item.ProductId, out var item);
            if (item == null)
            {
                item = Convert(itemAdded.Item);
                Cart.Add(item.ProductId, item);
            }
            else
            {
                item = new Com.Example.Shoppingcart.LineItem(item)
                {
                    Quantity = item.Quantity + itemAdded.Item.Quantity
                };
                Cart[item.ProductId] = item;
            }
        }

        [EventHandler(typeof(ItemRemoved))]
        public void itemRemoved(Com.Example.Shoppingcart.Persistence.ItemRemoved itemRemoved, IEventBehaviorContext c)
        {
            Cart.Remove(itemRemoved.ProductId);
        }

        [CommandHandler]
        public Com.Example.Shoppingcart.Cart GetCart()
        {
            var cart = new Com.Example.Shoppingcart.Cart();
            cart.Items.AddRange(Cart.Values);
            return cart;
        }

        [CommandHandler]
        public Empty AddItem(Com.Example.Shoppingcart.AddLineItem item, ICommandContext ctx)
        {
            if (item.Quantity <= 0)
            {
                ctx.Fail("Cannot add negative quantity of to item" + item.ProductId);
            }
            ctx.Emit(
                new Com.Example.Shoppingcart.Persistence.ItemAdded()
                {
                    Item = new Com.Example.Shoppingcart.Persistence.LineItem()
                    {
                        ProductId = item.ProductId,
                        Quantity = item.Quantity
                    }
                }
            );
            return new Empty();
        }

        Com.Example.Shoppingcart.LineItem Convert(Com.Example.Shoppingcart.Persistence.LineItem item)
        {
            var lineItem = new Com.Example.Shoppingcart.LineItem();
            lineItem.ProductId = item.ProductId;
            lineItem.Name = item.Name;
            lineItem.Quantity = item.Quantity;
            return lineItem;
        }

        Com.Example.Shoppingcart.Persistence.LineItem Convert(Com.Example.Shoppingcart.LineItem item) =>
          new Com.Example.Shoppingcart.Persistence.LineItem()
          {
              ProductId = item.ProductId,
              Name = item.Name,
              Quantity = item.Quantity
          };

    }
}
