using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Apworks.Repositories
{
    /// <summary>
    /// Represents that the implemented classes are repositories that stores the aggregate state in domain events
    /// and rebuilds the aggregate with either snapshots or domain events that was taken or raised on the aggregate
    /// which has the specified aggregate root key. The domain repository implemented the repository pattern as well
    /// as the concept of event sourcing in CQRS system architectures.
    /// </summary>
    public interface IDomainRepository : IDisposable
    {
        /// <summary>
        /// Gets the identifier of the current domain repository.
        /// </summary>
        Guid Id { get; }

        TAggregateRoot GetById<TKey, TAggregateRoot>(TKey id)
            where TKey : IEquatable<TKey>
            where TAggregateRoot : class, IAggregateRootWithEventSourcing<TKey>, new();

        void Save<TKey, TAggregateRoot>(TAggregateRoot aggregateRoot)
            where TKey : IEquatable<TKey>
            where TAggregateRoot : class, IAggregateRootWithEventSourcing<TKey>, new();

        Task<TAggregateRoot> GetByIdAsync<TKey, TAggregateRoot>(TKey id, CancellationToken cancellationToken = default(CancellationToken))
            where TKey : IEquatable<TKey>
            where TAggregateRoot : class, IAggregateRootWithEventSourcing<TKey>, new();

        Task SaveAsync<TKey, TAggregateRoot>(TAggregateRoot aggregateRoot, CancellationToken cancellationToken = default(CancellationToken))
            where TKey : IEquatable<TKey>
            where TAggregateRoot : class, IAggregateRootWithEventSourcing<TKey>, new();
    }
}
