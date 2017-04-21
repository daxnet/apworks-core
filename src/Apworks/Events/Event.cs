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
        public const string EventIntentMetadataKey = "apworks:event.intent";

        /// <summary>
        /// Initializes a new instance of the <see cref="Event"/> class.
        /// </summary>
        protected Event()
        {
            Metadata.Add(EventClrTypeMetadataKey, this.GetType().AssemblyQualifiedName);
            Metadata.Add(EventIntentMetadataKey, this.GetType().Name);
        }

        public string GetEventClrType() => this.Metadata[EventClrTypeMetadataKey].ToString();

        public string GetEventIntent() => this.Metadata[EventIntentMetadataKey].ToString();

        public virtual EventDescriptor ToDescriptor()
        {
            return new EventDescriptor
            {
                Id = Guid.NewGuid(),
                EventClrType = this.GetEventClrType(),
                EventId = this.Id,
                EventIntent = this.GetEventIntent(),
                EventTimestamp = this.Timestamp,
                EventPayload = this
            };
        }
    }
}
