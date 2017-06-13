using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Apworks.Messaging.RabbitMQ
{
    public class MessageBus : DisposableObject, IMessageBus
    {
        public event EventHandler<MessagePublishedEventArgs> MessagePublished;
        public event EventHandler<MessageReceivedEventArgs> MessageReceived;

        public void Publish<TMessage>(TMessage message) where TMessage : IMessage
        {
            throw new NotImplementedException();
        }

        public void PublishAll(IEnumerable<IMessage> messages)
        {
            throw new NotImplementedException();
        }

        public Task PublishAllAsync(IEnumerable<IMessage> messages, CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }

        public Task PublishAsync<TMessage>(TMessage message, CancellationToken cancellationToken = default(CancellationToken)) where TMessage : IMessage
        {
            throw new NotImplementedException();
        }

        public void Subscribe()
        {
            throw new NotImplementedException();
        }

        protected override void Dispose(bool disposing)
        {
            throw new NotImplementedException();
        }
    }
}
