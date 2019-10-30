using System;

namespace CloudState.CSharpSupport.Exceptions
{
    public class InvalidEntityConstructorParameterException : InvalidEntityConstructorException
    {
        public Type ArgumentType { get; }
        public InvalidEntityConstructorParameterException(Type type)
            : base($"Don't know how to handle argument of type {type.Name} in constructor")
        {
            ArgumentType = type;
        }
    }


}