using Apworks.Events;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apworks.Messaging.RabbitMQ
{
    public sealed class EventBus : MessageBus, IEventBus
    {
        public EventBus(string uri, IMessageSerializer messageSerializer, string exchangeName, string exchangeType = ExchangeType.Fanout, string queueName = null)
            : base(uri, messageSerializer, exchangeName, exchangeType, queueName)
        { }

        public EventBus(IConnectionFactory connectionFactory, IMessageSerializer messageSerializer, string exchangeName, string exchangeType = ExchangeType.Fanout, string queueName = null)
            : base(connectionFactory, messageSerializer, exchangeName, exchangeType, queueName)
        { }
    }
}
