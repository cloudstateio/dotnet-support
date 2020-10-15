// tag::main[]
using System.Threading.Tasks;
using Google.Protobuf;

namespace EventSourced.ShoppingCart
{
    public static class Program
    {
        public static async Task Main()
        {
            // tag::register[]
            var state = new CloudState.CSharpSupport.CloudState()
                    .RegisterEventSourcedEntity<ShoppingCartEntity>(
                        Com.Example.Shoppingcart.ShoppingCart.Descriptor,
                        Com.Example.Shoppingcart.Persistence.DomainReflection.Descriptor
                    );

            await state.StartAsync();
            // end::register[]
        }
    }
}
// end::main[]
