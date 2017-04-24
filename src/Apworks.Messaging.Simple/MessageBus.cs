using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Apworks.Messaging.Simple
{
    public class MessageBus : DisposableObject, IMessageBus
    {
        private readonly MessageQueue messageQueue = new MessageQueue();
        private bool subscribed = false;

        public event EventHandler<MessagePublishedEventArgs> MessagePublished;
        public event EventHandler<MessageReceivedEventArgs> MessageReceived;

        public void Publish<TMessage>(TMessage message) where TMessage : IMessage
        {
            messageQueue.PushMessage(message);
            this.OnMessagePublished(new MessagePublishedEventArgs(message));
        }

        public void PublishAll(IEnumerable<IMessage> messages) => messages.ToList().ForEach(msg => Publish(msg));

        public Task PublishAsync<TMessage>(TMessage message, CancellationToken cancellationToken = default(CancellationToken)) 
            where TMessage : IMessage => Task.Factory.StartNew(() => Publish(message), cancellationToken);

        public Task PublishAllAsync(IEnumerable<IMessage> messages, CancellationToken cancellationToken = default(CancellationToken)) => Task.Factory.StartNew(() => PublishAll(messages), cancellationToken);

        public void Subscribe()
        {
            if (!subscribed)
            {
                messageQueue.MessagePushed += (s, e) =>
                {
                    var message = ((MessageQueue)s).PopMessage();
                    this.OnMessageReceived(new MessageReceivedEventArgs(message));
                };
                subscribed = true;
            }
        }

        protected override void Dispose(bool disposing) { }

        private void OnMessagePublished(MessagePublishedEventArgs e)
        {
            this.MessagePublished?.Invoke(this, e);
        }

        private void OnMessageReceived(MessageReceivedEventArgs e)
        {
            this.MessageReceived?.Invoke(this, e);
        }
    }
}
