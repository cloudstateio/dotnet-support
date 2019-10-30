using System;

namespace CloudState.CSharpSupport.Exceptions
{
    public class MultipleEntityConstructorsFoundException : InvalidEntityConstructorException
    {
        public Type EntityType { get; }
        public MultipleEntityConstructorsFoundException(Type entityType)
            : base($"Only a single constructor is allowed on event sourced entities: {entityType}")
        {
            EntityType = entityType;
        }
    }


}