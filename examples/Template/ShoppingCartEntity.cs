using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using CloudState.CSharpSupport.Attributes;
using CloudState.CSharpSupport.Attributes.EventSourced;
using CloudState.CSharpSupport.Interfaces.EventSourced.Contexts;
using Com.Example.Shoppingcart.Persistence;
using Google.Protobuf.WellKnownTypes;

namespace CloudState.CSharpTemplate
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
        public Cart Snapshot()
        {
            var cart = new Cart();
            cart.Items.AddRange(Cart.Select(x => Convert(x.Value)));
            return cart;
        }

        [SnapshotHandler]
        public void HandleSnapshot(Cart cart)
        {
            Cart.Clear();
            foreach (LineItem item in cart.Items)
            {
                Cart.Add(item.ProductId, Convert(item));
            }
        }

        [EventHandler(typeof(ItemAdded))]
        public void ItemAdded(ItemAdded itemAdded)
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
        public void itemRemoved(ItemRemoved itemRemoved, IEventBehaviorContext c)
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
                // TODO: 
//                ctx.Fail("Cannot add negative quantity of to item" + item.ProductId);
            }
            ctx.Emit(
                new ItemAdded()
                {
                    Item = new LineItem()
                    {
                        ProductId = item.ProductId,
                        Quantity = item.Quantity
                    }
                }
            );
            return new Empty();
        }

        Com.Example.Shoppingcart.LineItem Convert(LineItem item)
        {
            var lineItem = new Com.Example.Shoppingcart.LineItem();
            lineItem.ProductId = item.ProductId;
            lineItem.Name = item.Name;
            lineItem.Quantity = item.Quantity;
            return lineItem;
        }

        LineItem Convert(Com.Example.Shoppingcart.LineItem item) =>
          new LineItem()
          {
              ProductId = item.ProductId,
              Name = item.Name,
              Quantity = item.Quantity
          };

    }
}
