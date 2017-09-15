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
        public const string EventIntentMetadataKey = "$apworks:event.intent";
        public const string EventOriginatorClrTypeMetadataKey = "$apworks:event.originatorClrtype";
        public const string EventOriginatorIdentifierMetadataKey = "$apworks:event.originatorId";

        /// <summary>
        /// Initializes a new instance of the <see cref="Event"/> class.
        /// </summary>
        protected Event()
        {
            Metadata[EventIntentMetadataKey] = this.GetType().Name;
        }

        public string GetEventIntent() => this.Metadata[EventIntentMetadataKey]?.ToString();

        public string GetOriginatorClrType() => this.Metadata[EventOriginatorClrTypeMetadataKey]?.ToString();

        public string GetOriginatorIdentifier() => this.Metadata[EventOriginatorIdentifierMetadataKey]?.ToString();

        public virtual EventDescriptor ToDescriptor()
        {
            return new EventDescriptor
            {
                Id = Guid.NewGuid(),
                EventClrType = this.GetMessageClrType(),
                EventId = this.Id,
                EventIntent = this.GetEventIntent(),
                EventTimestamp = this.Timestamp,
                OriginatorClrType = this.GetOriginatorClrType(),
                OriginatorId = this.GetOriginatorIdentifier(),
                EventPayload = this
            };
        }
    }
}
