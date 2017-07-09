using Apworks.Commands;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apworks.Messaging.RabbitMQ
{
    public sealed class CommandBus : MessageBus, ICommandBus
    {
        public CommandBus(string uri, IMessageSerializer messageSerializer, string exchangeName, string exchangeType = ExchangeType.Fanout, string queueName = null)
            : base(uri, messageSerializer, exchangeName, exchangeType, queueName)
        { }

        public CommandBus(IConnectionFactory connectionFactory, IMessageSerializer messageSerializer, string exchangeName, string exchangeType = ExchangeType.Fanout, string queueName = null)
            : base(connectionFactory, messageSerializer, exchangeName, exchangeType, queueName)
        { }
    }
}
