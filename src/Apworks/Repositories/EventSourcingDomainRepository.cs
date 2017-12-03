using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Apworks.Events;
using System.Linq;
using Apworks.Snapshots;

namespace Apworks.Repositories
{
    public sealed class EventSourcingDomainRepository : EventPublishingDomainRepository
    {
        private readonly IEventStore eventStore;
        private readonly ISnapshotProvider snapshotProvider;
        private readonly string route;
        private bool disposed = false;

        public EventSourcingDomainRepository(IEventStore eventStore, 
            IEventPublisher publisher,
            ISnapshotProvider snapshotProvider) 
            : this(eventStore, publisher, snapshotProvider, null)
        {

        }

        public EventSourcingDomainRepository(IEventStore eventStore,
            IEventPublisher publisher,
            ISnapshotProvider snapshotProvider,
            string route) : base(publisher)
        {
            this.eventStore = eventStore;
            this.snapshotProvider = snapshotProvider;
            this.route = route;
        }

        public override TAggregateRoot GetById<TKey, TAggregateRoot>(TKey id)
            => this.GetById<TKey, TAggregateRoot>(id, AggregateRootWithEventSourcing<TKey>.MaxVersion);

        public override TAggregateRoot GetById<TKey, TAggregateRoot>(TKey id, long version)
        {
            var sequenceMin = EventStore.MinimalSequence;
            var aggregateRoot = this.ActivateAggregateRoot<TKey, TAggregateRoot>();

            if (this.snapshotProvider.Enabled)
            {
                (var shouldCreate, var snapshot) = this.snapshotProvider.CheckSnapshot<TKey, TAggregateRoot>(id, version);
                if (snapshot != null)
                {
                    aggregateRoot.RestoreSnapshot(snapshot);
                    sequenceMin = snapshot.Version + 1;
                }
            }

            var events = this.eventStore.Load<TKey>(typeof(TAggregateRoot).AssemblyQualifiedName, id, sequenceMin, version);
            
            aggregateRoot.Replay(events.Select(e => e as IDomainEvent));
            return aggregateRoot;
        }

        public override async Task<TAggregateRoot> GetByIdAsync<TKey, TAggregateRoot>(TKey id, CancellationToken cancellationToken)
            => await this.GetByIdAsync<TKey, TAggregateRoot>(id, AggregateRootWithEventSourcing<TKey>.MaxVersion, cancellationToken);

        public override async Task<TAggregateRoot> GetByIdAsync<TKey, TAggregateRoot>(TKey id, long version, CancellationToken cancellationToken)
        {
            var sequenceMin = EventStore.MinimalSequence;
            var aggregateRoot = this.ActivateAggregateRoot<TKey, TAggregateRoot>();

            if (this.snapshotProvider.Enabled)
            {
                (var shouldCreate, var snapshot) = await this.snapshotProvider.CheckSnapshotAsync<TKey, TAggregateRoot>(id, version);
                if (snapshot != null)
                {
                    aggregateRoot.RestoreSnapshot(snapshot);
                    sequenceMin = snapshot.Version + 1;
                }
            }

            var events = await this.eventStore.LoadAsync<TKey>(typeof(TAggregateRoot).AssemblyQualifiedName,
                id,
                sequenceMin,
                version,
                cancellationToken: cancellationToken);
           
            aggregateRoot.Replay(events.Select(e => e as IDomainEvent));
            return aggregateRoot;
        }

        public override void Save<TKey, TAggregateRoot>(TAggregateRoot aggregateRoot)
        {
            // Saves the uncommitted events to the event store.
            var uncommittedEvents = aggregateRoot.UncommittedEvents;
            this.eventStore.Save(uncommittedEvents); // This will save the uncommitted events in a transaction.

            // Publishes the events.
            this.Publisher.PublishAll(uncommittedEvents, this.route);

            // Purges the uncommitted events.
            ((IPurgeable)aggregateRoot).Purge();

            // Checks and saves the snapshot.
            if (this.snapshotProvider.Enabled)
            {
                (var shouldCreate, var ss) = this.snapshotProvider.CheckSnapshot<TKey, TAggregateRoot>(aggregateRoot.Id, aggregateRoot.Version);
                if (shouldCreate)
                {
                    var snapshot = aggregateRoot.TakeSnapshot();
                    this.snapshotProvider.SaveSnapshot(snapshot);
                }
            }
        }

        public override async Task SaveAsync<TKey, TAggregateRoot>(TAggregateRoot aggregateRoot, CancellationToken cancellationToken)
        {
            // Saves the uncommitted events to the event store.
            var uncommittedEvents = aggregateRoot.UncommittedEvents;
            await this.eventStore.SaveAsync(uncommittedEvents, cancellationToken); // This will save the uncommitted events in a transaction.

            // Publishes the events.
            await this.Publisher.PublishAllAsync(uncommittedEvents, this.route, cancellationToken);

            // Purges the uncommitted events.
            ((IPurgeable)aggregateRoot).Purge();

            // Checks and saves the snapshot.
            if (this.snapshotProvider.Enabled)
            {
                (var shouldCreate, var ss) = await this.snapshotProvider.CheckSnapshotAsync<TKey, TAggregateRoot>(aggregateRoot.Id, aggregateRoot.Version, cancellationToken);
                if (shouldCreate)
                {
                    var snapshot = aggregateRoot.TakeSnapshot();
                    await this.snapshotProvider.SaveSnapshotAsync(snapshot, cancellationToken);
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    this.eventStore.Dispose();
                }

                disposed = true;
                base.Dispose(disposing);
            }
        }
    }
}
