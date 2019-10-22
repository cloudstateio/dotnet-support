using System;
using Cloudstate;
using Google.Protobuf.WellKnownTypes;
using io.cloudstate.csharpsupport.eventsourced;
using io.cloudstate.csharpsupport.impl.eventsourced;
using Optional;

namespace io.cloudstate.csharpsupport
{
    public class EventSourcedEntityCreationContext : IEventSourcedEntityCreationContext, IEventBehaviorContext
    {
        public string EntityId => throw new NotImplementedException();

        public IServiceCallFactory ServiceCallFactory => throw new NotImplementedException();

        Func<bool> IsActive { get; }

        Action<object[]> SetBehaviors { get; }

        public long SequenceNumber => throw new NotImplementedException();

        public EventSourcedEntityCreationContext(Func<bool> isActive, Action<object[]> setBehaviors)
        {
            IsActive = isActive;
            SetBehaviors = setBehaviors;
        }

        public void Become(params object[] behaviors)
        {
            if (!IsActive())
                throw new InvalidOperationException("Context is not active!");
            SetBehaviors(behaviors);
        }
    }

    public interface IAbstractClientActionContext : IClientActionContext
    {
        long CommandId { get; }

        Optional.Option<String> Error { get; internal set; }
        Optional.Option<Forward> ForwardMessage { get; protected set; }

        bool HasError { get { return Error.HasValue; } }

        new Exception Fail(string errorMessage)
        {
            ((IActivateableContext)this).CheckActive();
            if (!Error.HasValue)
            {
                Error = Optional.Option.Some(errorMessage);
                throw new FailInvokedException();
            }
            else
            {
                throw new InvalidOperationException("fail(…) already previously invoked!");
            }
        }

        new void Forward(IServiceCall to)
        {
            ((IActivateableContext)this).CheckActive();
            if (ForwardMessage.HasValue)
            {
                throw new InvalidOperationException("This context has already forwarded.");
            }
            ForwardMessage = Optional.Option.Some(
                new Forward()
                {
                    ServiceName = to.GetRef().Method.Service.FullName,
                    CommandName = to.GetRef().Method.Name,
                    Payload = Any.Pack(to.Message)
                }
            );
        }

        Option<ClientAction> CreateClientAction(Option<Any> reply, bool allowNoReply) =>
            Error.Match(
                some: msg =>
                    Optional.Option.Some(new ClientAction()
                    {
                        Failure = new Failure()
                        {
                            CommandId = CommandId,
                            Description = msg
                        }
                    }),
                none: () =>
                {
                    if (reply.HasValue)
                    {
                        if (ForwardMessage.HasValue)
                        {
                            throw new InvalidOperationException("Both a reply was returned, and a forward message was sent, choose one or the other.");
                        }
                        return Optional.Option.Some(
                            new ClientAction()
                            {
                                Reply = new Reply()
                                {
                                    Payload = reply.Match(
                                        some: x => x,
                                        none: () => throw new NullReferenceException(nameof(reply))
                                    )
                                }
                            }
                        );
                    }
                    else if (ForwardMessage.HasValue)
                    {
                        return Optional.Option.Some(
                            new ClientAction()
                            {
                                Forward = ForwardMessage.Match(
                                    some: x => x,
                                    none: () => throw new NullReferenceException(nameof(ForwardMessage))
                                )
                            }
                        );
                    }
                    else if (allowNoReply)
                    {
                        return Optional.Option.None<ClientAction>();
                    }
                    else
                    {
                        throw new Exception("No reply or forward returned by command handler");
                    }
                }
            );

    }


}