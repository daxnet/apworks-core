using Apworks.Snapshots;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Apworks.Tests.Helpers
{
    internal class InMemorySnapshotProvider : ISnapshotProvider
    {
        public readonly List<ISnapshot> snapshots = new List<ISnapshot>();

        public bool Enabled => true;

        public (bool, ISnapshot) CheckSnapshot<TKey, TAggregateRoot>(TKey key, long version)
        {
            var latestSnapshot = this.snapshots.OrderByDescending(_ => _.Version)
                .FirstOrDefault(x => x.Version <= version);
            if (latestSnapshot == null)
            {
                return (true, null);
            }

            return (version - latestSnapshot.Version >= 30, latestSnapshot);
        }

        public Task<(bool, ISnapshot)> CheckSnapshotAsync<TKey, TAggregateRoot>(TKey key, long version, CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }

        public void SaveSnapshot(ISnapshot snapshot)
        {
            snapshots.Add(snapshot);
        }

        public Task SaveSnapshotAsync(ISnapshot snapshot, CancellationToken cancellationToken = default(CancellationToken))
            => Task.Factory.StartNew(_ => SaveSnapshot(snapshot), cancellationToken);

        //public ISnapshot GetLatestSnapshot<TKey, TAggregateRoot>(TKey id, long version)
        //    where TKey : IEquatable<TKey>
        //    where TAggregateRoot : class, IAggregateRootWithEventSourcing<TKey>
        //{
        //    return this.snapshots.OrderByDescending(x => x.Version)
        //        .FirstOrDefault(x => x.Version <= version);
        //}

        //public Task<ISnapshot> GetLatestSnapshotAsync<TKey, TAggregateRoot>(TKey id, long version, CancellationToken cancellationToken)
        //    where TKey : IEquatable<TKey>
        //    where TAggregateRoot : class, IAggregateRootWithEventSourcing<TKey>
        //    => Task.FromResult(GetLatestSnapshot<TKey, TAggregateRoot>(id, version));

        //public bool ShouldSaveSnapshot<TKey, TAggregateRoot>(TAggregateRoot aggregateRoot)
        //    where TKey : IEquatable<TKey>
        //    where TAggregateRoot : class, IAggregateRootWithEventSourcing<TKey>
        //{
        //    var snapshot = this.GetLatestSnapshot<TKey, TAggregateRoot>(aggregateRoot.Id, aggregateRoot.Version);

        //}
    }
}
