using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Apworks.Snapshots
{
    /// <summary>
    /// Represents the snapshot provider that does nothing when the domain repository
    /// is going to manipulate the snapshots for a particular aggregate.
    /// </summary>
    public sealed class SuppressedSnapshotProvider : ISnapshotProvider
    {
        public bool Enabled => false;

        public void SaveSnapshot(ISnapshot snapshot) { }

        public Task SaveSnapshotAsync(ISnapshot snapshot, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.CompletedTask;
        }

        public bool ShouldSaveSnapshot<TKey, TAggregateRoot>(TAggregateRoot aggregateRoot)
            where TKey : IEquatable<TKey>
            where TAggregateRoot : class, IAggregateRootWithEventSourcing<TKey>
        {
            return false;
        }

        public ISnapshot GetLatestSnapshot<TKey, TAggregateRoot>(long version)
            where TKey : IEquatable<TKey>
            where TAggregateRoot : class, IAggregateRootWithEventSourcing<TKey>
                => null;

        public Task<ISnapshot> GetLatestSnapshotAsync<TKey, TAggregateRoot>(long version, CancellationToken cancellationToken)
            where TKey : IEquatable<TKey>
            where TAggregateRoot : class, IAggregateRootWithEventSourcing<TKey>
                => Task.FromResult<ISnapshot>(null);
    }
}
