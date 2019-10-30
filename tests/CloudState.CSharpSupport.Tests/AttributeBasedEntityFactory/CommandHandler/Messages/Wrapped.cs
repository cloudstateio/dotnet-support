using System;

namespace CloudState.CSharpSupport.Tests.AttributeBasedEntityFactory.CommandHandler.Messages
{

    public class Wrapped
    {
        public string Value { get; }
        public Wrapped(string value)
        {
            Value = value;
        }

        public override bool Equals(object obj)
        {
            switch (obj)
            {
                case Wrapped other:
                    return string.Equals(other.Value, Value);
                default:
                    return false;
            }
        }

        protected bool Equals(Wrapped other)
        {
            return Value == other.Value;
        }

        public override int GetHashCode()
        {
            return (Value != null ? Value.GetHashCode() : 0);
        }
    }

}
