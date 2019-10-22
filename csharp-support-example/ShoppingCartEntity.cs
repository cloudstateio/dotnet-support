using System;
using System.Collections.Generic;
using System.Linq;
using Com.Example.Shoppingcart.Persistence;
using Google.Protobuf.WellKnownTypes;
using io.cloudstate.csharpsupport;
using io.cloudstate.csharpsupport.eventsourced;
using EventHandlerAttribute = io.cloudstate.csharpsupport.eventsourced.EventHandlerAttribute;

namespace io.cloudstate.samples.shoppingCart
{

    [EventSourcedEntity]
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
        public void itemRemoved(Com.Example.Shoppingcart.Persistence.ItemRemoved itemRemoved)
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

        [CommandHandler]
        public Empty removeItem(Com.Example.Shoppingcart.RemoveLineItem item, ICommandContext ctx)
        {
            if (!Cart.ContainsKey(item.ProductId))
            {
                ctx.Fail("Cannot remove item " + item.ProductId + " because it is not in the cart.");
            }
            ctx.Emit(new Com.Example.Shoppingcart.Persistence.ItemRemoved() { ProductId = item.ProductId });
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