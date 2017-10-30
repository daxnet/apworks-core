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
        public EventConsumer(IEventSubscriber subscriber, IEnumerable<IEventHandler> handlers, string route = null) 
            : base(subscriber, handlers, route)
        {
        }
    }
}
