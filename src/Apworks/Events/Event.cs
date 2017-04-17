using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Apworks.Messaging;

namespace Apworks.Events
{
    /// <summary>
    /// Represents the base class for events.
    /// </summary>
    /// <seealso cref="Apworks.Messaging.Message" />
    /// <seealso cref="Apworks.Events.IEvent" />
    public abstract class Event : Message, IEvent
    {
        public const string EventClrTypeMetadataKey = "apworks:event.clrtype";
        public const string EventNameMetadataKey = "apworks:event.name";

        /// <summary>
        /// Initializes a new instance of the <see cref="Event"/> class.
        /// </summary>
        protected Event()
        {
            Metadata.Add(EventClrTypeMetadataKey, this.GetType().AssemblyQualifiedName);
            Metadata.Add(EventNameMetadataKey, this.GetType().Name);
        }
    }
}
