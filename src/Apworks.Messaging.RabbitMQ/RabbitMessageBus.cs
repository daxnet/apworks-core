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
    public class RabbitMessageBus : MessageBus
    {
        #region Fields
        private readonly IConnectionFactory connectionFactory;
        private readonly IConnection connection;
        private readonly IModel channel;
        private readonly string exchangeName;
        private readonly string queueName;
        private readonly string exchangeType;
        private readonly bool autoAck;
        private bool disposed;
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="RabbitMessageBus"/> class.
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="messageSerializer"></param>
        /// <param name="exchangeName"></param>
        /// <param name="exchangeType"></param>
        /// <param name="queueName"></param>
        /// <param name="autoAck"></param>
        public RabbitMessageBus(string uri, 
            IMessageSerializer messageSerializer,
            IMessageHandlerExecutionContext messageHandlerExecutionContext,
            string exchangeName, 
            string exchangeType = ExchangeType.Fanout, 
            string queueName = null,
            bool autoAck = false)
            : this(new ConnectionFactory { Uri = new Uri(uri) }, 
                  messageSerializer, 
                  messageHandlerExecutionContext,
                  exchangeName, 
                  exchangeType, 
                  queueName, 
                  autoAck)
        { }

        public RabbitMessageBus(IConnectionFactory connectionFactory, 
            IMessageSerializer messageSerializer,
            IMessageHandlerExecutionContext messageHandlerExecutionContext,
            string exchangeName, 
            string exchangeType = ExchangeType.Fanout, 
            string queueName = null,
            bool autoAck = false)
            : base(messageSerializer, messageHandlerExecutionContext)
        {
            // Initializes the local variables
            this.connectionFactory = connectionFactory;
            this.connection = connectionFactory.CreateConnection();
            this.channel = connection.CreateModel();
            this.exchangeType = exchangeType;
            this.exchangeName = exchangeName;
            this.autoAck = autoAck;

            // Declares the exchange
            this.channel.ExchangeDeclare(this.exchangeName, this.exchangeType);

            // Initializes the consumer of the message queue
            this.queueName = this.InitializeMessageConsumer(queueName);
        }

        protected override void DoPublish<TMessage>(TMessage message)
        {
            var messageBody = this.MessageSerializer.Serialize(message);
            channel.BasicPublish(this.exchangeName,
                message.GetType().FullName,
                null,
                messageBody);
        }

        protected override async Task DoPublishAsync<TMessage>(TMessage message, CancellationToken cancellationToken = default(CancellationToken))
        {
            var messageBody = await this.MessageSerializer.SerializeAsync(message, cancellationToken);
            channel.BasicPublish(this.exchangeName,
                message.GetType().FullName,
                null,
                messageBody);
        }

        public override void Subscribe<TMessage, TMessageHandler>()
        {
            if (!this.MessageHandlerExecutionContext.HandlerRegistered<TMessage, TMessageHandler>())
            {
                this.MessageHandlerExecutionContext.RegisterHandler<TMessage, TMessageHandler>();

                this.channel.QueueBind(this.queueName, this.exchangeName, typeof(TMessage).FullName);
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

        private string InitializeMessageConsumer(string queue)
        {
            var localQueueName = queue;
            if (string.IsNullOrEmpty(localQueueName))
            {
                localQueueName = this.channel.QueueDeclare().QueueName;
            }
            else
            {
                this.channel.QueueDeclare(localQueueName, true, false, false, null);
            }

            var consumer = new EventingBasicConsumer(this.channel);
            consumer.Received += async (model, eventArgument) =>
            {
                var messageBody = eventArgument.Body;
                var message = this.MessageSerializer.Deserialize(messageBody);
                this.OnMessageReceived(new MessageReceivedEventArgs(message, this.MessageSerializer));
                await this.MessageHandlerExecutionContext.HandleMessageAsync(message);
                if (!autoAck)
                {
                    channel.BasicAck(eventArgument.DeliveryTag, false);
                }

                this.OnMessageAcknowledged(new MessageAcknowledgedEventArgs(message, this.MessageSerializer, this.autoAck));
            };

            this.channel.BasicConsume(localQueueName, autoAck: this.autoAck, consumer: consumer);

            return localQueueName;
        }
    }
}
