using System.Threading.Tasks;
using Com.Example.Crdts;

namespace Crdt.Example
{
    public static class Program
    {
        public static async Task Main()
        {
            var state = new CloudState.CSharpSupport.CloudState()
                .RegisterCrdtEntity<CrdtExampleEntity>(
                    CrdtExample.Descriptor,
                    CrdtExampleReflection.Descriptor);

            await state.StartAsync();
        }
    }
}
