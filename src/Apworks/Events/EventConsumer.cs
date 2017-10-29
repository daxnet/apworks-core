using Apworks.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apworks.Events
{
    public class EventConsumer : MessageConsumer<IEventSubscriber, IEventHandler>, IEventConsumer
    {
        protected EventConsumer(IEventSubscriber subscriber, IEnumerable<IEventHandler> handlers) 
            : base(subscriber, handlers)
        {
        }
    }
}
