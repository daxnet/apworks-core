using Apworks.Events;
using System;
using System.Linq;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Apworks.EventStore.Dictionary
{
    public class DictionaryEventStore : Events.EventStore
    {
        private readonly ConcurrentDictionary<string, List<EventDescriptor>> bank = new ConcurrentDictionary<string, List<EventDescriptor>>();

        public override IEnumerable<EventDescriptor> Load<TKey>(string originatorClrType, TKey originatorId)
        {
            var key = $"{originatorClrType}_{originatorId.ToString()}";
            bank.TryGetValue(key, out List<EventDescriptor> descriptors);
            return descriptors;
        }

        public override void Save(IEnumerable<EventDescriptor> eventDescriptors)
        {
            var query = from p in eventDescriptors
                        where !string.IsNullOrEmpty(p.OriginatorClrType) &&
                                !string.IsNullOrEmpty(p.OriginatorId)
                        group p by new { p.OriginatorClrType, p.OriginatorId } into g
                        select new { Key = g.Key, Values = g.ToList() };

            foreach(var item in query)
            {
                this.bank.AddOrUpdate($"{item.Key.OriginatorClrType}_{item.Key.OriginatorId}",
                    item.Values, (k, origin) => { origin.AddRange(item.Values); return origin; });
            }
        }
    }
}
