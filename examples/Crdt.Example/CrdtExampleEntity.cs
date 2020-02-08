using System;
using CloudState.CSharpSupport.Attributes.Crdt;
using CloudState.CSharpSupport.Interfaces.Crdt.Contexts;
using CloudState.CSharpSupport.Interfaces.Crdt.Elements;
using Com.Example.Crdts;

namespace Crdt.Example
{
    [CrdtEntity]
    public class CrdtExampleEntity
    {
        [CommandHandler]
        public CounterValue IncrementGCounter(UpdateCounter update, ICommandContext ctx)
        {
            if (update.Value < 0)
            {
                ctx.Fail("Cannot decrement IGCounter");
            }
            
            return ctx.State<IGCounter>(typeof(IGCounter))
                .Match(
                    x =>
                    {
                        if (update.Value > 0)
                            x.Increment(update.Value);
                        return new CounterValue
                        {
                            Value = x.Value
                        };
                    },
                    () => throw new InvalidOperationException("No state found."));

        }
        
    }
}