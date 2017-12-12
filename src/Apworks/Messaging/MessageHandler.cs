using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Apworks.Messaging
{
    public abstract class MessageHandler : IMessageHandler
    {
        public abstract bool CanHandle(Type messageType);

        public abstract Task<bool> HandleAsync(IMessage message, CancellationToken cancellationToken = default(CancellationToken));
    }

    public abstract class MessageHandler<TMessage> : MessageHandler, IMessageHandler<TMessage>
        where TMessage : IMessage
    {
        public sealed override bool CanHandle(Type messageType)
        {
            return typeof(TMessage).Equals(messageType);
        }

        public sealed override async Task<bool> HandleAsync(IMessage message, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (message == null)
            {
                return false;
            }

            if (message is TMessage)
            {
                return await this.HandleAsync((TMessage)message, cancellationToken);
            }

            return false;
        }

        public abstract Task<bool> HandleAsync(TMessage message, CancellationToken cancellationToken = default(CancellationToken));
    }
}
