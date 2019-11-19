using System;

namespace CloudState.CSharpSupport.Attributes.Crdt
{
  [AttributeUsage(AttributeTargets.Method)]
  public class CommandHandlerAttribute : CloudStateAttribute {
    public string Name { get; }
  }
}

