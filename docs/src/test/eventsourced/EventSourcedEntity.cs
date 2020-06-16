
// #constructing
public ShoppingCartEntity([EntityId]String entityId)
{
    EntityId = entityId;
    Cart = new Dictionary<string, Com.Example.Shoppingcart.LineItem>();
}
// #constructing

// #entity-class
namespace EventSourced.ShoppingCart
{
    [EventSourcedEntity]
    public class ShoppingCartEntity
    {
        // ...
    }
// #entity-class

    // #entity-state
    Dictionary<String, Com.Example.Shoppingcart.LineItem> Cart { get; }
    // #entity-state

    // #snapshot
    [Snapshot]
    public Cart Snapshot()
    {
        var cart = new Cart();
        cart.Items.AddRange(Cart.Select(x => Convert(x.Value)));
        return cart;
    }

    Com.Example.Shoppingcart.LineItem Convert(LineItem item)
    {
        var lineItem = new Com.Example.Shoppingcart.LineItem();
        lineItem.ProductId = item.ProductId;
        lineItem.Name = item.Name;
        lineItem.Quantity = item.Quantity;
        return lineItem;
    }
    // #snapshot

    // #handle-snapshot
    [SnapshotHandler]
    public void HandleSnapshot(Cart cart)
    {
        Cart.Clear();
        foreach (LineItem item in cart.Items)
        {
            Cart.Add(item.ProductId, Convert(item));
        }
    }

    LineItem Convert(Com.Example.Shoppingcart.LineItem item) =>
    new LineItem()
    {
        ProductId = item.ProductId,
        Name = item.Name,
        Quantity = item.Quantity
    };
    // #handle-snapshot

    // #item-added
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
    // #item-added

    // #item-removed
    [EventHandler(typeof(ItemRemoved))]
    public void itemRemoved(ItemRemoved itemRemoved, IEventBehaviorContext c)
    {
        Cart.Remove(itemRemoved.ProductId);
    }
    // #item-removed

    // #get-cart
    [CommandHandler]
    public Com.Example.Shoppingcart.Cart GetCart()
    {
        var cart = new Com.Example.Shoppingcart.Cart();
        cart.Items.AddRange(Cart.Values);
        return cart;
    }
    // #get-cart

    // #add-item
    [CommandHandler]
    public Empty AddItem(Com.Example.Shoppingcart.AddLineItem item, ICommandContext ctx)
    {
        if (item.Quantity <= 0)
        {
            ctx.Fail("Cannot add negative quantity of to item" + item.ProductId);
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
    // #add-item

}