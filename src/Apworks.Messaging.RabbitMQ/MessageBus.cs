using RabbitMQ.Client;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using RabbitMQ.Client.Events;

namespace Apworks.Messaging.RabbitMQ
{
    public class MessageBus : DisposableObject, IMessageBus
    {
        private readonly IConnectionFactory connectionFactory;
        private readonly IObjectSerializer messageSerializer;
        private readonly IConnection connection;
        private readonly IModel channel;
        private readonly string exchangeName;
        private readonly string queueName;
        private readonly string exchangeType;
        private bool subscribed;

        private bool disposed;

        public MessageBus(string uri, IObjectSerializer messageSerializer, string exchangeName, string exchangeType = ExchangeType.Fanout, string queueName = null)
            : this(new ConnectionFactory { Uri = uri }, messageSerializer, exchangeName, exchangeType, queueName)
        { }

        public MessageBus(IConnectionFactory connectionFactory, IObjectSerializer messageSerializer, string exchangeName, string exchangeType = ExchangeType.Fanout, string queueName = null)
        {
            // Initializes the local variables
            this.connectionFactory = connectionFactory;
            this.messageSerializer = messageSerializer;
            this.connection = connectionFactory.CreateConnection();
            this.channel = connection.CreateModel();
            this.exchangeType = exchangeType;
            this.exchangeName = exchangeName;
            this.queueName = queueName;

            // Declares the exchanges
            this.channel.ExchangeDeclare(this.exchangeName, this.exchangeType);
        }

        public event EventHandler<MessagePublishedEventArgs> MessagePublished;
        public event EventHandler<MessageReceivedEventArgs> MessageReceived;
        public event EventHandler<MessageProcessedEventArgs> MessageAcknowledged;

        public void Publish<TMessage>(TMessage message, string route = null) where TMessage : IMessage
        {
            var messageBody = this.messageSerializer.Serialize(message);
            channel.BasicPublish(this.exchangeName,
                route ?? string.Empty,
                null,
                messageBody);
            this.OnMessagePublished(new MessagePublishedEventArgs(message));
        }

        public void PublishAll(IEnumerable<IMessage> messages, string route = null)
        {
            messages.ToList().ForEach(m => this.Publish(m, route));
        }

        public async Task PublishAllAsync(IEnumerable<IMessage> messages, string route = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            foreach (var message in messages)
            {
                await this.PublishAsync(message, route, cancellationToken);
            }
        }

        public async Task PublishAsync<TMessage>(TMessage message, string route = null, CancellationToken cancellationToken = default(CancellationToken)) where TMessage : IMessage
        {
            var messageBody = await this.messageSerializer.SerializeAsync(message, cancellationToken);
            channel.BasicPublish(this.exchangeName,
                route ?? string.Empty,
                null,
                messageBody);
            this.OnMessagePublished(new MessagePublishedEventArgs(message));
        }

        public void Subscribe(string route = null)
        {
            if (!this.subscribed)
            {
                var queue = this.queueName;
                if (string.IsNullOrEmpty(this.queueName))
                {
                    queue = this.channel.QueueDeclare().QueueName;
                }
                else
                {
                    this.channel.QueueDeclare(queue, true, false, false, null);
                }

                this.channel.QueueBind(queue, this.exchangeName, route ?? string.Empty);
                var consumer = new EventingBasicConsumer(this.channel);
                consumer.Received += (model, eventArgument) =>
                  {
                      var messageBody = eventArgument.Body;
                      var message = (IMessage)this.messageSerializer.Deserialize(messageBody);
                      this.OnMessageReceived(new MessageReceivedEventArgs(message));
                      channel.BasicAck(eventArgument.DeliveryTag, false);
                      this.OnMessageAcknowledged(new MessageProcessedEventArgs(message));
                  };

                this.channel.BasicConsume(queue, false, consumer);

                this.subscribed = true;
            }
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (!disposed)
                {
                    this.channel.Dispose();
                    this.connection.Dispose();
                    disposed = true;
                }
            }
        }

        private void OnMessagePublished(MessagePublishedEventArgs e)
        {
            this.MessagePublished?.Invoke(this, e);
        }

        private void OnMessageReceived(MessageReceivedEventArgs e)
        {
            this.MessageReceived?.Invoke(this, e);
        }

        private void OnMessageAcknowledged(MessageProcessedEventArgs e)
        {
            this.MessageAcknowledged?.Invoke(this, e);
        }
    }
}
