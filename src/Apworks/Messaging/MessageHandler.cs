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
        public abstract bool Handle(object message);

        public virtual Task<bool> HandleAsync(object message, CancellationToken cancellationToken = default(CancellationToken)) => Task.FromResult(Handle(message));
    }

    public abstract class MessageHandler<TMessage> : MessageHandler, IMessageHandler<TMessage>
        where TMessage : IMessage
    {
        public override bool Handle(object message)
        {
            if (message == null)
            {
                return false;
            }

            if  (message is TMessage)
            {
                return this.Handle((TMessage)message);
            }

            return false;
        }

        public override async Task<bool> HandleAsync(object message, CancellationToken cancellationToken = default(CancellationToken))
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

        public abstract bool Handle(TMessage message);

        public virtual Task<bool> HandleAsync(TMessage message, CancellationToken cancellationToken = default(CancellationToken)) => Task.FromResult(Handle(message));
    }
}
