using System.Collections.Generic;
using CloudState.CSharpSupport.Interfaces.Contexts;
using CloudState.CSharpSupport.Reflection.Interfaces;

namespace CloudState.CSharpSupport.Interfaces
{
    internal interface IStatefulEntityService<in TContext, out THandler> : IStatefulService
        where TContext : IEntityContext
        where THandler : IEntityHandler
    {
        THandler CreateEntityHandler(TContext ctx);
    }

    internal interface IResolvedEntityFactory
    {
        IReadOnlyDictionary<string, IResolvedServiceMethod> ResolvedMethods { get; }
    }
}