using Apworks.Events;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apworks.Messaging.RabbitMQ
{
    public class RabbitEventBus : RabbitMessageBus, IEventBus
    {
        public RabbitEventBus(string uri, 
            IMessageSerializer messageSerializer, 
            string exchangeName, 
            string exchangeType = ExchangeType.Fanout, 
            string queueName = null,
            bool autoAck = false)
            : base(uri, messageSerializer, exchangeName, exchangeType, queueName, autoAck)
        { }

        public RabbitEventBus(IConnectionFactory connectionFactory, 
            IMessageSerializer messageSerializer, 
            string exchangeName, 
            string exchangeType = ExchangeType.Fanout, 
            string queueName = null,
            bool autoAck = false)
            : base(connectionFactory, messageSerializer, exchangeName, exchangeType, queueName, autoAck)
        { }
    }
}
