using Apworks.Events;
using System;
using System.Collections.Generic;

namespace Apworks
{
    /// <summary>
    /// Represents that the implemented classes are aggregate roots that support
    /// event sourcing (ES) capability.
    /// </summary>
    /// <typeparam name="TKey">The type of the aggregate root key.</typeparam>
    /// <seealso cref="Apworks.IAggregateRoot{TKey}" />
    public interface IAggregateRootWithEventSourcing<TKey> : IAggregateRoot<TKey>, IPurgeable
        where TKey : IEquatable<TKey>
    {
        /// <summary>
        /// Gets the uncommitted domain events stored within the current aggregate.
        /// </summary>
        /// <value>
        /// The uncommitted events.
        /// </value>
        IEnumerable<IDomainEvent> UncommittedEvents { get; }

        /// <summary>
        /// Replays the domain events one by one to restore the aggregate state.
        /// </summary>
        /// <param name="domainEvents">The domain events to be replayed on the current aggregate.</param>
        void Replay(IEnumerable<IDomainEvent> domainEvents);

        /// <summary>
        /// Gets the version of current aggregate root.
        /// </summary>
        /// <value>
        /// The version.
        /// </value>
        long Version { get; }
    }
}
