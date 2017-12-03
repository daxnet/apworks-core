using RabbitMQ.Client;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using RabbitMQ.Client.Events;
using System.Text;

namespace Apworks.Messaging.RabbitMQ
{
    /// <summary>
    /// Represents the message bus implemented with RabbitMQ.
    /// </summary>
    /// <remarks>
    /// For more information about RabbitMQ, please refer to: https://www.rabbitmq.com.
    /// </remarks>
    public class MessageBus : DisposableObject, IMessageBus
    {
        #region Fields
        private readonly IConnectionFactory connectionFactory;
        private readonly IMessageSerializer messageSerializer;
        private readonly IConnection connection;
        private readonly IModel channel;
        private readonly string exchangeName;
        private readonly string queueName;
        private readonly string exchangeType;
        private readonly bool autoAck;

        private bool subscribed;
        private bool disposed;
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageBus"/> class.
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="messageSerializer"></param>
        /// <param name="exchangeName"></param>
        /// <param name="exchangeType"></param>
        /// <param name="queueName"></param>
        /// <param name="autoAck"></param>
        public MessageBus(string uri, 
            IMessageSerializer messageSerializer, 
            string exchangeName, 
            string exchangeType = ExchangeType.Fanout, 
            string queueName = null,
            bool autoAck = false)
            : this(new ConnectionFactory { Uri = new Uri(uri) }, messageSerializer, exchangeName, exchangeType, queueName, autoAck)
        { }

        public MessageBus(IConnectionFactory connectionFactory, 
            IMessageSerializer messageSerializer, 
            string exchangeName, 
            string exchangeType = ExchangeType.Fanout, 
            string queueName = null,
            bool autoAck = false)
        {
            // Initializes the local variables
            this.connectionFactory = connectionFactory;
            this.messageSerializer = messageSerializer;
            this.connection = connectionFactory.CreateConnection();
            this.channel = connection.CreateModel();
            this.exchangeType = exchangeType;
            this.exchangeName = exchangeName;
            this.queueName = queueName;
            this.autoAck = autoAck;

            // Declares the exchanges
            this.channel.ExchangeDeclare(this.exchangeName, this.exchangeType);
        }

        public event EventHandler<MessagePublishedEventArgs> MessagePublished;
        public event EventHandler<MessageReceivedEventArgs> MessageReceived;
        public event EventHandler<MessageAcknowledgedEventArgs> MessageAcknowledged;

        public void Publish<TMessage>(TMessage message, string route = null) where TMessage : IMessage
        {
            var messageBody = this.messageSerializer.Serialize(message);
            channel.BasicPublish(this.exchangeName,
                route ?? string.Empty,
                null,
                messageBody);
            this.OnMessagePublished(new MessagePublishedEventArgs(message, this.messageSerializer));
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
            this.OnMessagePublished(new MessagePublishedEventArgs(message, this.messageSerializer));
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
                      var message = this.messageSerializer.Deserialize(messageBody);
                      this.OnMessageReceived(new MessageReceivedEventArgs(message, this.messageSerializer));
                      if (!autoAck)
                      {
                          channel.BasicAck(eventArgument.DeliveryTag, false);
                      }

                      this.OnMessageAcknowledged(new MessageAcknowledgedEventArgs(message, this.messageSerializer, this.autoAck));
                  };

                this.channel.BasicConsume(queue, autoAck: this.autoAck, consumer: consumer);

                this.subscribed = true;
            }
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    this.channel.Dispose();
                    this.connection.Dispose();
                }

                disposed = true;
                base.Dispose(disposing);
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

        private void OnMessageAcknowledged(MessageAcknowledgedEventArgs e)
        {
            this.MessageAcknowledged?.Invoke(this, e);
        }
    }
}
