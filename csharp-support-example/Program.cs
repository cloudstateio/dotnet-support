using System.Threading.Tasks;
using io.cloudstate.csharpsupport;
using io.cloudstate.samples.shoppingCart;

namespace csharp_support_example
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            var state = new CloudState()
                    .RegisterEventSourcedEntity<ShoppingCartEntity>(
                        Com.Example.Shoppingcart.ShoppingCart.Descriptor,
                        Com.Example.Shoppingcart.Persistence.DomainReflection.Descriptor
                    );

            await state.StartAsync();
        }
    }
}
