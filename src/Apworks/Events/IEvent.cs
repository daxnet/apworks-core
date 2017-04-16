using System;

namespace Apworks.Events
{
    /// <summary>
    /// Represents that the implemented classes are events.
    /// </summary>
    public interface IEvent
    {
        /// <summary>
        /// Gets or sets the identifier of the event.
        /// </summary>
        /// <value>
        /// The identifier of the event.
        /// </value>
        Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the timestamp which describes when the current event occurs.
        /// </summary>
        /// <value>
        /// The timestamp which describes when the current event occurs.
        /// </value>
        DateTime Timestamp { get; set; }
    }
}
