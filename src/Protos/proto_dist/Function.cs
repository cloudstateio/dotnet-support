// <auto-generated>
//     Generated by the protocol buffer compiler.  DO NOT EDIT!
//     source: cloudstate/function.proto
// </auto-generated>
#pragma warning disable 1591, 0612, 3021
#region Designer generated code

using pb = global::Google.Protobuf;
using pbc = global::Google.Protobuf.Collections;
using pbr = global::Google.Protobuf.Reflection;
using scg = global::System.Collections.Generic;
namespace Cloudstate.Function {

  /// <summary>Holder for reflection information generated from cloudstate/function.proto</summary>
  public static partial class FunctionReflection {

    #region Descriptor
    /// <summary>File descriptor for cloudstate/function.proto</summary>
    public static pbr::FileDescriptor Descriptor {
      get { return descriptor; }
    }
    private static pbr::FileDescriptor descriptor;

    static FunctionReflection() {
      byte[] descriptorData = global::System.Convert.FromBase64String(
          string.Concat(
            "ChljbG91ZHN0YXRlL2Z1bmN0aW9uLnByb3RvEhNjbG91ZHN0YXRlLmZ1bmN0",
            "aW9uGhlnb29nbGUvcHJvdG9idWYvYW55LnByb3RvGhdjbG91ZHN0YXRlL2Vu",
            "dGl0eS5wcm90byJcCg9GdW5jdGlvbkNvbW1hbmQSFAoMc2VydmljZV9uYW1l",
            "GAIgASgJEgwKBG5hbWUYAyABKAkSJQoHcGF5bG9hZBgEIAEoCzIULmdvb2ds",
            "ZS5wcm90b2J1Zi5BbnkilQEKDUZ1bmN0aW9uUmVwbHkSIgoFcmVwbHkYAiAB",
            "KAsyES5jbG91ZHN0YXRlLlJlcGx5SAASJgoHZm9yd2FyZBgDIAEoCzITLmNs",
            "b3Vkc3RhdGUuRm9yd2FyZEgAEiwKDHNpZGVfZWZmZWN0cxgEIAMoCzIWLmNs",
            "b3Vkc3RhdGUuU2lkZUVmZmVjdEIKCghyZXNwb25zZTKVAwoRU3RhdGVsZXNz",
            "RnVuY3Rpb24SWQoLaGFuZGxlVW5hcnkSJC5jbG91ZHN0YXRlLmZ1bmN0aW9u",
            "LkZ1bmN0aW9uQ29tbWFuZBoiLmNsb3Vkc3RhdGUuZnVuY3Rpb24uRnVuY3Rp",
            "b25SZXBseSIAEmAKEGhhbmRsZVN0cmVhbWVkSW4SJC5jbG91ZHN0YXRlLmZ1",
            "bmN0aW9uLkZ1bmN0aW9uQ29tbWFuZBoiLmNsb3Vkc3RhdGUuZnVuY3Rpb24u",
            "RnVuY3Rpb25SZXBseSIAKAESYQoRaGFuZGxlU3RyZWFtZWRPdXQSJC5jbG91",
            "ZHN0YXRlLmZ1bmN0aW9uLkZ1bmN0aW9uQ29tbWFuZBoiLmNsb3Vkc3RhdGUu",
            "ZnVuY3Rpb24uRnVuY3Rpb25SZXBseSIAMAESYAoOaGFuZGxlU3RyZWFtZWQS",
            "JC5jbG91ZHN0YXRlLmZ1bmN0aW9uLkZ1bmN0aW9uQ29tbWFuZBoiLmNsb3Vk",
            "c3RhdGUuZnVuY3Rpb24uRnVuY3Rpb25SZXBseSIAKAEwAUIYChZpby5jbG91",
            "ZHN0YXRlLnByb3RvY29sYgZwcm90bzM="));
      descriptor = pbr::FileDescriptor.FromGeneratedCode(descriptorData,
          new pbr::FileDescriptor[] { global::Google.Protobuf.WellKnownTypes.AnyReflection.Descriptor, global::Cloudstate.EntityReflection.Descriptor, },
          new pbr::GeneratedClrTypeInfo(null, new pbr::GeneratedClrTypeInfo[] {
            new pbr::GeneratedClrTypeInfo(typeof(global::Cloudstate.Function.FunctionCommand), global::Cloudstate.Function.FunctionCommand.Parser, new[]{ "ServiceName", "Name", "Payload" }, null, null, null),
            new pbr::GeneratedClrTypeInfo(typeof(global::Cloudstate.Function.FunctionReply), global::Cloudstate.Function.FunctionReply.Parser, new[]{ "Reply", "Forward", "SideEffects" }, new[]{ "Response" }, null, null)
          }));
    }
    #endregion

  }
  #region Messages
  public sealed partial class FunctionCommand : pb::IMessage<FunctionCommand> {
    private static readonly pb::MessageParser<FunctionCommand> _parser = new pb::MessageParser<FunctionCommand>(() => new FunctionCommand());
    private pb::UnknownFieldSet _unknownFields;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<FunctionCommand> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::Cloudstate.Function.FunctionReflection.Descriptor.MessageTypes[0]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public FunctionCommand() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public FunctionCommand(FunctionCommand other) : this() {
      serviceName_ = other.serviceName_;
      name_ = other.name_;
      payload_ = other.payload_ != null ? other.payload_.Clone() : null;
      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public FunctionCommand Clone() {
      return new FunctionCommand(this);
    }

    /// <summary>Field number for the "service_name" field.</summary>
    public const int ServiceNameFieldNumber = 2;
    private string serviceName_ = "";
    /// <summary>
    /// The name of the service this function is on.
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public string ServiceName {
      get { return serviceName_; }
      set {
        serviceName_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    /// <summary>Field number for the "name" field.</summary>
    public const int NameFieldNumber = 3;
    private string name_ = "";
    /// <summary>
    /// Command name
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public string Name {
      get { return name_; }
      set {
        name_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    /// <summary>Field number for the "payload" field.</summary>
    public const int PayloadFieldNumber = 4;
    private global::Google.Protobuf.WellKnownTypes.Any payload_;
    /// <summary>
    /// The command payload.
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public global::Google.Protobuf.WellKnownTypes.Any Payload {
      get { return payload_; }
      set {
        payload_ = value;
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override bool Equals(object other) {
      return Equals(other as FunctionCommand);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool Equals(FunctionCommand other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (ServiceName != other.ServiceName) return false;
      if (Name != other.Name) return false;
      if (!object.Equals(Payload, other.Payload)) return false;
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override int GetHashCode() {
      int hash = 1;
      if (ServiceName.Length != 0) hash ^= ServiceName.GetHashCode();
      if (Name.Length != 0) hash ^= Name.GetHashCode();
      if (payload_ != null) hash ^= Payload.GetHashCode();
      if (_unknownFields != null) {
        hash ^= _unknownFields.GetHashCode();
      }
      return hash;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override string ToString() {
      return pb::JsonFormatter.ToDiagnosticString(this);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
      if (ServiceName.Length != 0) {
        output.WriteRawTag(18);
        output.WriteString(ServiceName);
      }
      if (Name.Length != 0) {
        output.WriteRawTag(26);
        output.WriteString(Name);
      }
      if (payload_ != null) {
        output.WriteRawTag(34);
        output.WriteMessage(Payload);
      }
      if (_unknownFields != null) {
        _unknownFields.WriteTo(output);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (ServiceName.Length != 0) {
        size += 1 + pb::CodedOutputStream.ComputeStringSize(ServiceName);
      }
      if (Name.Length != 0) {
        size += 1 + pb::CodedOutputStream.ComputeStringSize(Name);
      }
      if (payload_ != null) {
        size += 1 + pb::CodedOutputStream.ComputeMessageSize(Payload);
      }
      if (_unknownFields != null) {
        size += _unknownFields.CalculateSize();
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(FunctionCommand other) {
      if (other == null) {
        return;
      }
      if (other.ServiceName.Length != 0) {
        ServiceName = other.ServiceName;
      }
      if (other.Name.Length != 0) {
        Name = other.Name;
      }
      if (other.payload_ != null) {
        if (payload_ == null) {
          Payload = new global::Google.Protobuf.WellKnownTypes.Any();
        }
        Payload.MergeFrom(other.Payload);
      }
      _unknownFields = pb::UnknownFieldSet.MergeFrom(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(pb::CodedInputStream input) {
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, input);
            break;
          case 18: {
            ServiceName = input.ReadString();
            break;
          }
          case 26: {
            Name = input.ReadString();
            break;
          }
          case 34: {
            if (payload_ == null) {
              Payload = new global::Google.Protobuf.WellKnownTypes.Any();
            }
            input.ReadMessage(Payload);
            break;
          }
        }
      }
    }

  }

  public sealed partial class FunctionReply : pb::IMessage<FunctionReply> {
    private static readonly pb::MessageParser<FunctionReply> _parser = new pb::MessageParser<FunctionReply>(() => new FunctionReply());
    private pb::UnknownFieldSet _unknownFields;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<FunctionReply> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::Cloudstate.Function.FunctionReflection.Descriptor.MessageTypes[1]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public FunctionReply() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public FunctionReply(FunctionReply other) : this() {
      sideEffects_ = other.sideEffects_.Clone();
      switch (other.ResponseCase) {
        case ResponseOneofCase.Reply:
          Reply = other.Reply.Clone();
          break;
        case ResponseOneofCase.Forward:
          Forward = other.Forward.Clone();
          break;
      }

      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public FunctionReply Clone() {
      return new FunctionReply(this);
    }

    /// <summary>Field number for the "reply" field.</summary>
    public const int ReplyFieldNumber = 2;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public global::Cloudstate.Reply Reply {
      get { return responseCase_ == ResponseOneofCase.Reply ? (global::Cloudstate.Reply) response_ : null; }
      set {
        response_ = value;
        responseCase_ = value == null ? ResponseOneofCase.None : ResponseOneofCase.Reply;
      }
    }

    /// <summary>Field number for the "forward" field.</summary>
    public const int ForwardFieldNumber = 3;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public global::Cloudstate.Forward Forward {
      get { return responseCase_ == ResponseOneofCase.Forward ? (global::Cloudstate.Forward) response_ : null; }
      set {
        response_ = value;
        responseCase_ = value == null ? ResponseOneofCase.None : ResponseOneofCase.Forward;
      }
    }

    /// <summary>Field number for the "side_effects" field.</summary>
    public const int SideEffectsFieldNumber = 4;
    private static readonly pb::FieldCodec<global::Cloudstate.SideEffect> _repeated_sideEffects_codec
        = pb::FieldCodec.ForMessage(34, global::Cloudstate.SideEffect.Parser);
    private readonly pbc::RepeatedField<global::Cloudstate.SideEffect> sideEffects_ = new pbc::RepeatedField<global::Cloudstate.SideEffect>();
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public pbc::RepeatedField<global::Cloudstate.SideEffect> SideEffects {
      get { return sideEffects_; }
    }

    private object response_;
    /// <summary>Enum of possible cases for the "response" oneof.</summary>
    public enum ResponseOneofCase {
      None = 0,
      Reply = 2,
      Forward = 3,
    }
    private ResponseOneofCase responseCase_ = ResponseOneofCase.None;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public ResponseOneofCase ResponseCase {
      get { return responseCase_; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void ClearResponse() {
      responseCase_ = ResponseOneofCase.None;
      response_ = null;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override bool Equals(object other) {
      return Equals(other as FunctionReply);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool Equals(FunctionReply other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (!object.Equals(Reply, other.Reply)) return false;
      if (!object.Equals(Forward, other.Forward)) return false;
      if(!sideEffects_.Equals(other.sideEffects_)) return false;
      if (ResponseCase != other.ResponseCase) return false;
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override int GetHashCode() {
      int hash = 1;
      if (responseCase_ == ResponseOneofCase.Reply) hash ^= Reply.GetHashCode();
      if (responseCase_ == ResponseOneofCase.Forward) hash ^= Forward.GetHashCode();
      hash ^= sideEffects_.GetHashCode();
      hash ^= (int) responseCase_;
      if (_unknownFields != null) {
        hash ^= _unknownFields.GetHashCode();
      }
      return hash;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override string ToString() {
      return pb::JsonFormatter.ToDiagnosticString(this);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
      if (responseCase_ == ResponseOneofCase.Reply) {
        output.WriteRawTag(18);
        output.WriteMessage(Reply);
      }
      if (responseCase_ == ResponseOneofCase.Forward) {
        output.WriteRawTag(26);
        output.WriteMessage(Forward);
      }
      sideEffects_.WriteTo(output, _repeated_sideEffects_codec);
      if (_unknownFields != null) {
        _unknownFields.WriteTo(output);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (responseCase_ == ResponseOneofCase.Reply) {
        size += 1 + pb::CodedOutputStream.ComputeMessageSize(Reply);
      }
      if (responseCase_ == ResponseOneofCase.Forward) {
        size += 1 + pb::CodedOutputStream.ComputeMessageSize(Forward);
      }
      size += sideEffects_.CalculateSize(_repeated_sideEffects_codec);
      if (_unknownFields != null) {
        size += _unknownFields.CalculateSize();
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(FunctionReply other) {
      if (other == null) {
        return;
      }
      sideEffects_.Add(other.sideEffects_);
      switch (other.ResponseCase) {
        case ResponseOneofCase.Reply:
          if (Reply == null) {
            Reply = new global::Cloudstate.Reply();
          }
          Reply.MergeFrom(other.Reply);
          break;
        case ResponseOneofCase.Forward:
          if (Forward == null) {
            Forward = new global::Cloudstate.Forward();
          }
          Forward.MergeFrom(other.Forward);
          break;
      }

      _unknownFields = pb::UnknownFieldSet.MergeFrom(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(pb::CodedInputStream input) {
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, input);
            break;
          case 18: {
            global::Cloudstate.Reply subBuilder = new global::Cloudstate.Reply();
            if (responseCase_ == ResponseOneofCase.Reply) {
              subBuilder.MergeFrom(Reply);
            }
            input.ReadMessage(subBuilder);
            Reply = subBuilder;
            break;
          }
          case 26: {
            global::Cloudstate.Forward subBuilder = new global::Cloudstate.Forward();
            if (responseCase_ == ResponseOneofCase.Forward) {
              subBuilder.MergeFrom(Forward);
            }
            input.ReadMessage(subBuilder);
            Forward = subBuilder;
            break;
          }
          case 34: {
            sideEffects_.AddEntriesFrom(input, _repeated_sideEffects_codec);
            break;
          }
        }
      }
    }

  }

  #endregion

}

#endregion Designer generated code
