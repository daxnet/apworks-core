using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Apworks.Events;

namespace Apworks.Repositories
{
    public sealed class EventSourcingDomainRepository : EventPublishingDomainRepository
    {
        private readonly IEventStore eventStore;

        public EventSourcingDomainRepository(IEventStore eventStore, IEventPublisher publisher) : base(publisher)
        {
            this.eventStore = eventStore;
        }

        public override TAggregateRoot GetById<TKey, TAggregateRoot>(TKey id)
        {
            var events = this.eventStore.Load<TKey>(typeof(TAggregateRoot).AssemblyQualifiedName, id) as IEnumerable<IDomainEvent>;
            var aggregateRoot = new TAggregateRoot();
            aggregateRoot.Replay(events);
            return aggregateRoot;
        }

        public override async Task<TAggregateRoot> GetByIdAsync<TKey, TAggregateRoot>(TKey id, CancellationToken cancellationToken)
        {
            var events = await this.eventStore.LoadAsync<TKey>(typeof(TAggregateRoot).AssemblyQualifiedName, id) as IEnumerable<IDomainEvent>;
            var aggregateRoot = new TAggregateRoot();
            aggregateRoot.Replay(events);
            return aggregateRoot;
        }

        public override void Save<TKey, TAggregateRoot>(TAggregateRoot aggregateRoot)
        {
            // Saves the uncommitted events to the event store.
            var uncommittedEvents = aggregateRoot.UncommittedEvents;
            this.eventStore.Save(uncommittedEvents); // This will save the uncommitted events in a transaction.

            // Publishes the events.
            this.Publisher.PublishAll(uncommittedEvents);

            // Purges the uncommitted events.
            ((IPurgeable)aggregateRoot).Purge();
        }

        public override async Task SaveAsync<TKey, TAggregateRoot>(TAggregateRoot aggregateRoot, CancellationToken cancellationToken)
        {
            // Saves the uncommitted events to the event store.
            var uncommittedEvents = aggregateRoot.UncommittedEvents;
            await this.eventStore.SaveAsync(uncommittedEvents, cancellationToken); // This will save the uncommitted events in a transaction.

            // Publishes the events.
            await this.Publisher.PublishAllAsync(uncommittedEvents);

            // Purges the uncommitted events.
            ((IPurgeable)aggregateRoot).Purge();
        }
    }
}
