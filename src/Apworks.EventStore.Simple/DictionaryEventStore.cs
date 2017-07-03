using Apworks.Events;
using System;
using System.Linq;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Apworks.EventStore.Simple
{
    public class DictionaryEventStore : Events.EventStore
    {
        private readonly ConcurrentDictionary<string, List<EventDescriptor>> bank = new ConcurrentDictionary<string, List<EventDescriptor>>();

        public DictionaryEventStore()
            : base(new DummySerializer())
        { }

        protected override IEnumerable<EventDescriptor> LoadDescriptors<TKey>(string originatorClrType, TKey originatorId, long sequenceMin, long sequenceMax)
        {
            var key = $"{originatorClrType}_{originatorId.ToString()}";
            bank.TryGetValue(key, out List<EventDescriptor> descriptors);
            return descriptors.OrderBy(d => d.EventSequence).Where(d => d.EventSequence >= sequenceMin && d.EventSequence <= sequenceMax);
        }

        protected override void SaveDescriptors(IEnumerable<EventDescriptor> eventDescriptors)
        {
            var query = from p in eventDescriptors
                        where !string.IsNullOrEmpty(p.OriginatorClrType) &&
                                !string.IsNullOrEmpty(p.OriginatorId)
                        orderby p.EventTimestamp ascending
                        group p by new { p.OriginatorClrType, p.OriginatorId } into g
                        select new { Key = g.Key, Values = g.ToList() };

            foreach (var item in query)
            {
                this.bank.AddOrUpdate($"{item.Key.OriginatorClrType}_{item.Key.OriginatorId}",
                    item.Values, (k, origin) => { origin.AddRange(item.Values); return origin; });
            }
        }
    }

    internal sealed class DummySerializer : ObjectSerializer
    {
        public override object Deserialize(Type objType, byte[] data)
        {
            throw new NotImplementedException();
        }

        public override object Deserialize(byte[] data)
        {
            throw new NotImplementedException();
        }

        public override byte[] Serialize(Type objType, object obj)
        {
            throw new NotImplementedException();
        }

        public override byte[] Serialize(object @object)
        {
            throw new NotImplementedException();
        }
    }
}
