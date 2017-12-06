using Apworks.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apworks.Commands
{
    public class CommandConsumer : MessageConsumer<ICommandSubscriber>, ICommandConsumer
    {
        public CommandConsumer(ICommandSubscriber subscriber, IMessageHandlerManager messageHandlerManager) 
            : base(subscriber, messageHandlerManager)
        {
        }
    }
}
