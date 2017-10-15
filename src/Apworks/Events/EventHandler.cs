using Apworks.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Apworks.Events
{
    /// <summary>
    /// Represents the base class for event handlers.
    /// </summary>
    /// <typeparam name="TEvent">The type of the event.</typeparam>
    /// <seealso cref="Apworks.Messaging.MessageHandler{TEvent}" />
    /// <seealso cref="Apworks.Events.IEventHandler{TEvent}" />
    public abstract class EventHandler<TEvent> : MessageHandler<TEvent>, IEventHandler<TEvent>
        where TEvent : IEvent
    {

    }
}
