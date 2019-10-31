using System;
using Cloudstate;
using CloudState.CSharpSupport.Exceptions;
using CloudState.CSharpSupport.Interfaces.Contexts;
using CloudState.CSharpSupport.Interfaces.Services;
using Google.Protobuf.WellKnownTypes;
using Optional;

namespace CloudState.CSharpSupport.Contexts.Abstractions
{
    internal class AbstractClientActionContext : IClientActionContext
    {
        private AbstractActivatableContext ActivatableContext { get; }
        private IContext RootContext { get; }

        long CommandId { get; }

        private Option<string> Error { get; set; }
        private Option<Forward> ForwardMessage { get; set; }

        public bool HasError => Error.HasValue;

        public IServiceCallFactory ServiceCallFactory => RootContext.ServiceCallFactory;

        internal AbstractClientActionContext(long commandId, IContext rootContext, AbstractActivatableContext activatableContext)
        {
            RootContext = rootContext;
            ActivatableContext = activatableContext;
            CommandId = commandId;
        }

        public Exception Fail(string errorMessage)
        {
            ActivatableContext.CheckActive();
            if (!Error.HasValue)
            {
                Error = Optional.Option.Some(errorMessage);
                throw new FailInvokedException();
            }
            throw new InvalidOperationException("fail(…) already previously invoked!");
        }

        public void Forward(IServiceCall to)
        {
            ActivatableContext.CheckActive();
            if (ForwardMessage.HasValue)
                throw new InvalidOperationException("This context has already forwarded.");

            ForwardMessage = Optional.Option.Some(
                new Forward
                {
                    ServiceName = to.Ref.Method.Service.FullName,
                    CommandName = to.Ref.Method.Name,
                    Payload = Any.Pack(to.Message)
                }
            );
        }

        public Option<ClientAction> CreateClientAction(Option<Any> reply, bool allowNoReply) =>
            Error.Match(
                some: msg =>
                    Optional.Option.Some(new ClientAction
                    {
                        Failure = new Failure
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
                            new ClientAction
                            {
                                Reply = new Reply
                                {
                                    Payload = reply.Match(
                                        some: x => x,
                                        none: () => throw new ArgumentNullException(nameof(reply))
                                    )
                                }
                            }
                        );
                    }
                    else if (ForwardMessage.HasValue)
                    {
                        return Optional.Option.Some(
                            new ClientAction
                            {
                                Forward = ForwardMessage.Match(
                                    some: x => x,
                                    none: () => throw new ArgumentNullException(nameof(ForwardMessage))
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