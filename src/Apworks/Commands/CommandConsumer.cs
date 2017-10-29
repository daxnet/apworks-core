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
        protected CommandConsumer(ICommandSubscriber subscriber, IEnumerable<ICommandHandler> handlers) 
            : base(subscriber, handlers)
        {
        }
    }
}
