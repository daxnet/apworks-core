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
        public abstract bool Handle(IMessage message);

        public abstract bool CanHandle(Type messageType);

        public virtual Task<bool> HandleAsync(IMessage message, CancellationToken cancellationToken = default(CancellationToken)) => Task.FromResult(Handle(message));
    }

    public abstract class MessageHandler<TMessage> : MessageHandler, IMessageHandler<TMessage>
        where TMessage : IMessage
    {
        public sealed override bool CanHandle(Type messageType)
        {
            return typeof(TMessage).Equals(messageType);
        }

        public override bool Handle(IMessage message)
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

        public override async Task<bool> HandleAsync(IMessage message, CancellationToken cancellationToken = default(CancellationToken))
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

        public virtual bool Handle(TMessage message) => this.HandleAsync(message).Result;

        public abstract Task<bool> HandleAsync(TMessage message, CancellationToken cancellationToken = default(CancellationToken));
    }
}
