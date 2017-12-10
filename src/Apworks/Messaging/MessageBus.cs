using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Apworks.Messaging
{
    public abstract class MessageBus : DisposableObject, IMessageBus
    {
        private readonly IMessageSerializer messageSerializer;
        private readonly IMessageHandlerExecutionContext messageHandlerExecutionContext;

        protected MessageBus(IMessageSerializer messageSerializer,
            IMessageHandlerExecutionContext messageHandlerExecutionContext)
        {
            this.messageSerializer = messageSerializer;
            this.messageHandlerExecutionContext = messageHandlerExecutionContext;
        }

        public event EventHandler<MessagePublishedEventArgs> MessagePublished;
        public event EventHandler<MessageReceivedEventArgs> MessageReceived;
        public event EventHandler<MessageAcknowledgedEventArgs> MessageAcknowledged;

        public virtual void Publish<TMessage>(TMessage message) where TMessage : IMessage
        {
            this.DoPublish(message);
            this.OnMessagePublished(new MessagePublishedEventArgs(message, this.messageSerializer));
        }

        public virtual async Task PublishAsync<TMessage>(TMessage message,
            CancellationToken cancellationToken = default(CancellationToken)) where TMessage : IMessage
        {
            await this.DoPublishAsync(message, cancellationToken);
            this.OnMessagePublished(new MessagePublishedEventArgs(message, this.messageSerializer));
        }

        public virtual void PublishAll(IEnumerable<IMessage> messages)
            => messages.ToList().ForEach(m => Publish(m));

        public async virtual Task PublishAllAsync(IEnumerable<IMessage> messages, 
            CancellationToken cancellationToken = default(CancellationToken))
        {
            foreach (var message in messages)
            {
                await this.PublishAsync(message, cancellationToken);
            }
        }

        public abstract void Subscribe<TMessage, TMessageHandler>()
            where TMessage : IMessage
            where TMessageHandler : IMessageHandler<TMessage>;

        public IMessageSerializer MessageSerializer => this.messageSerializer;

        public IMessageHandlerExecutionContext MessageHandlerExecutionContext => this.messageHandlerExecutionContext;

        protected abstract void DoPublish<TMessage>(TMessage message)
            where TMessage : IMessage;

        protected abstract Task DoPublishAsync<TMessage>(TMessage message, CancellationToken cancellationToken = default(CancellationToken))
            where TMessage : IMessage;

        protected virtual void OnMessagePublished(MessagePublishedEventArgs e) => this.MessagePublished?.Invoke(this, e);

        protected virtual void OnMessageReceived(MessageReceivedEventArgs e) => this.MessageReceived?.Invoke(this, e);

        protected virtual void OnMessageAcknowledged(MessageAcknowledgedEventArgs e) => this.MessageAcknowledged?.Invoke(this, e);
    }
}
