using Apworks.Messaging;
using System;

namespace Apworks.Events
{
    /// <summary>
    /// Represents that the implemented classes are events.
    /// </summary>
    public interface IEvent : IMessage
    {
        /// <summary>
        /// Gets the CLR type (a.k.a. assembly qualified type) of the current event.
        /// </summary>
        /// <value>
        /// The CLR type of the current event.
        /// </value>
        string GetEventClrType();

        /// <summary>
        /// Gets the intent of the current event. Usually it is the simplified name
        /// of the <c>ClrType</c>.
        /// </summary>
        /// <value>
        /// The intent of the current event.
        /// </value>
        string GetEventIntent();

        /// <summary>
        /// Gets the CLR type (a.k.a. assembly qualified type) of the originator of the current event.
        /// </summary>
        /// <returns>The CLR type of the originator.</returns>
        string GetOriginatorClrType();

        /// <summary>
        /// Gets the originator identifier.
        /// </summary>
        /// <returns></returns>
        string GetOriginatorIdentifier();

        /// <summary>
        /// Converts the current <see cref="IEvent"/> instance to a <see cref="EventDescriptor"/>.
        /// </summary>
        /// <returns>The event descriptor.</returns>
        EventDescriptor ToDescriptor();
    }
}
