using Apworks.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apworks.Messaging.Simple
{
    public sealed class SimpleEventBus : SimpleMessageBus, IEventBus
    {
        public SimpleEventBus(IMessageSerializer messageSerializer, IMessageHandlerExecutionContext messageHandlerExecutionContext)
            : base(messageSerializer, messageHandlerExecutionContext)
        { }
    }
}
