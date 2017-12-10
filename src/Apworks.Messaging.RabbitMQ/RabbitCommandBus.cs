using Apworks.Commands;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apworks.Messaging.RabbitMQ
{
    public class RabbitCommandBus : RabbitMessageBus, ICommandBus
    {
        public RabbitCommandBus(string uri, 
            IMessageSerializer messageSerializer, 
            IMessageHandlerExecutionContext messageHandlerExecutionContext,
            string exchangeName, 
            string exchangeType = ExchangeType.Fanout, 
            string queueName = null,
            bool autoAck = false)
            : base(uri, messageSerializer, messageHandlerExecutionContext, exchangeName, exchangeType, queueName, autoAck)
        { }

        public RabbitCommandBus(IConnectionFactory connectionFactory, 
            IMessageSerializer messageSerializer,
            IMessageHandlerExecutionContext messageHandlerExecutionContext,
            string exchangeName, 
            string exchangeType = ExchangeType.Fanout, 
            string queueName = null,
            bool autoAck = false)
            : base(connectionFactory, messageSerializer, messageHandlerExecutionContext, exchangeName, exchangeType, queueName, autoAck)
        { }
    }
}
