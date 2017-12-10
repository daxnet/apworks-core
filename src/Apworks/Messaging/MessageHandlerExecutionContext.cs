using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Apworks.Messaging
{
    public abstract class MessageHandlerExecutionContext : IMessageHandlerExecutionContext
    {
        public void RegisterHandler<TMessage, THandler>()
            where TMessage : IMessage
            where THandler : IMessageHandler<TMessage>
            => RegisterHandler(typeof(TMessage), typeof(THandler));

        public bool HandlerRegistered<TMessage, THandler>()
            where TMessage : IMessage
            where THandler : IMessageHandler<TMessage>
            => HandlerRegistered(typeof(TMessage), typeof(THandler));

        public abstract void RegisterHandler(Type messageType, Type handlerType);

        public abstract bool HandlerRegistered(Type messageType, Type handlerType);

        public abstract Task HandleMessageAsync(IMessage message, CancellationToken cancellationToken = default(CancellationToken));
    }
}
