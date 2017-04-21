using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apworks.Events
{
    /// <summary>
    /// Represents the data that holds the event metadata and payload.
    /// </summary>
    /// <seealso cref="Apworks.IAggregateRoot{System.Guid}" />
    public sealed class EventDescriptor : IAggregateRoot<Guid>
    {
        /// <summary>
        /// Gets or sets the identifier of the current <see cref="EventDescriptor"/>.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public Guid Id { get; set; }


        public Guid EventId { get; set; }

        public DateTime EventTimestamp { get; set; }

        public string EventClrType { get; set; }

        public string EventIntent { get; set; }

        public string OriginatorClrType { get; set; }

        public string OriginatorId { get; set; }

        public object EventPayload { get; set; }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString() => this.EventIntent;
    }
}
