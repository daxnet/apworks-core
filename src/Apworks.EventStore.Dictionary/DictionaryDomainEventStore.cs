using Apworks.Events;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Apworks.EventStore.Dictionary
{
    public class DictionaryDomainEventStore : DomainEventStore
    {
        private readonly ConcurrentDictionary<string, List<IDomainEvent>> bank = new ConcurrentDictionary<string, List<IDomainEvent>>();

        public override IEnumerable<IDomainEvent> Load<TKey>(string aggregateRootTypeIdentifier, TKey id)
        {
            throw new NotImplementedException();
        }

        public override void Save(IEnumerable<IDomainEvent> events)
        {
            throw new NotImplementedException();
        }
    }
}
