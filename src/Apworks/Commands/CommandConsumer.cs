using Apworks.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apworks.Commands
{
    public class CommandConsumer : MessageConsumer<ICommandSubscriber, ICommandHandler>, ICommandConsumer
    {
        public CommandConsumer(ICommandSubscriber subscriber, IEnumerable<ICommandHandler> handlers, string route = null) 
            : base(subscriber, handlers, route)
        {
        }
    }
}
