using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            throw new NotImplementedException();
        }

        public override void Save<TKey, TAggregateRoot>(TAggregateRoot aggregateRoot)
        {
            throw new NotImplementedException();
        }
    }
}
