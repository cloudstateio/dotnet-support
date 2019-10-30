using System.Threading.Tasks;

namespace csharp_support
{
    public static class Program
    {
        public static async Task Main()
        {
            var state = new CloudState.CSharpSupport.CloudState()
                    .RegisterEventSourcedEntity<ShoppingCartEntity>(
                        Com.Example.Shoppingcart.ShoppingCart.Descriptor,
                        Com.Example.Shoppingcart.Persistence.DomainReflection.Descriptor
                    );

            await state.StartAsync();
        }
    }
}
