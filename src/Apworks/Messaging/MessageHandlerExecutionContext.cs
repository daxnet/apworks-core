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
        //public IEnumerable<IMessageHandler> GetHandlersFor<TMessage>() where TMessage : IMessage
        //{
        //    return GetHandlersFor(typeof(TMessage));
        //}

        //public IEnumerable<IMessageHandler> GetHandlersFor(Type messageType)
        //    => ResolveHandler(messageType);

        //public bool HasHandlersRegistered<TMessage>() where TMessage : IMessage
        //    => HasHandlersRegistered(typeof(TMessage));

        //public abstract bool HasHandlersRegistered(Type messageType);

        //public bool HasRegistered<TMessage, THandler>()
        //    where TMessage : IMessage
        //    where THandler : IMessageHandler<TMessage>
        //    => HasRegistered(typeof(TMessage), typeof(THandler));

        //public abstract bool HasRegistered(Type messageType, Type handlerType);

        public void RegisterHandler<TMessage, THandler>()
            where TMessage : IMessage
            where THandler : IMessageHandler<TMessage>
            => RegisterHandler(typeof(TMessage), typeof(THandler));

        public abstract void RegisterHandler(Type messageType, Type handlerType);

        //protected abstract IEnumerable<IMessageHandler> ResolveHandler(Type messageType);

        public abstract Task HandleMessageAsync(IMessage message, CancellationToken cancellationToken = default(CancellationToken));
    }
}
