﻿using Apworks.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apworks.Events
{
    /// <summary>
    /// Represents that the implemented classes are event handlers.
    /// </summary>
    /// <typeparam name="TEvent">The type of the event that will be handled by the current handler.</typeparam>
    public interface IEventHandler<in TEvent> : IMessageHandler<TEvent>
        where TEvent : IEvent
    {
    }
}
