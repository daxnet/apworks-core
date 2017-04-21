using Apworks.Events;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Apworks.EventStore.Dictionary
{
    public class DictionaryEventStore : Events.EventStore
    {
        private readonly ConcurrentDictionary<string, List<EventDescriptor>> bank = new ConcurrentDictionary<string, List<EventDescriptor>>();

        public override IEnumerable<EventDescriptor> Load<TKey>(string aggregateRootTypeIdentifier, TKey id)
        {
            bank.TryGetValue(ConcatKey(aggregateRootTypeIdentifier, id), out List<EventDescriptor> descriptors);
            return descriptors;
        }

        public override void Save(IEnumerable<EventDescriptor> events)
        {
            foreach(var eventDescriptor in events)
            {

            }
        }

        private static string ConcatKey<TKey>(string aggregateRootTypeIdentifier, TKey id)
        {
            return $"{aggregateRootTypeIdentifier}_{id.ToString()}";
        }
    }
}
