﻿using System;
using System.Threading.Tasks;
using csharp_support;

namespace Template
{
    class Program
    {
        static async Task Main(string[] args)
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
