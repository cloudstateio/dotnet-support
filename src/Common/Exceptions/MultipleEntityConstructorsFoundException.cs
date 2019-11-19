using System;

namespace CloudState.CSharpSupport.Exceptions
{
    public class MultipleEntityConstructorsFoundException : InvalidEntityConstructorException
    {
        public Type EntityType { get; }
        public Type EntityClass { get; }
        public MultipleEntityConstructorsFoundException(Type entityClass, Type entityType)
            : base($"Only a single constructor is allowed on {entityClass} entities: {entityType}")
        {
            EntityType = entityType;
            EntityClass = entityClass;
        }
    }


}