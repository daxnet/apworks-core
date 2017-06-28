using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Apworks.Events
{
    public abstract class EventStore : IEventStore
    {
        public const long MinimalSequence = -1L;
        public const long MaximumSequence = long.MaxValue;

        private readonly IObjectSerializer payloadSerializer;

        protected EventStore(IObjectSerializer payloadSerializer)
        {
            this.payloadSerializer = payloadSerializer;
        }

        protected IObjectSerializer PayloadSerializer { get => this.payloadSerializer; }

        public virtual void Dispose() { }

        public IEnumerable<IEvent> Load<TKey>(string originatorClrType, TKey originatorId, long sequenceMin = EventStore.MinimalSequence, long sequenceMax = EventStore.MaximumSequence) 
            where TKey : IEquatable<TKey>
        {
            var descriptors = this.LoadDescriptors<TKey>(originatorClrType, originatorId, sequenceMin, sequenceMax);
            foreach(var descriptor in descriptors)
            {
                if (descriptor.EventPayload != null &&
                    descriptor.EventPayload is IEvent)
                {
                    yield return (IEvent)descriptor.EventPayload;
                }
            }
            yield break;
        }

        public async Task<IEnumerable<IEvent>> LoadAsync<TKey>(string originatorClrType, TKey originatorId, long sequenceMin = EventStore.MinimalSequence, long sequenceMax = EventStore.MaximumSequence, CancellationToken cancellationToken = default(CancellationToken))
            where TKey : IEquatable<TKey>
        {
            var descriptors = await this.LoadDescriptorsAsync<TKey>(originatorClrType, originatorId, sequenceMin, sequenceMax, cancellationToken);
            List<IEvent> events = new List<IEvent>();
            foreach(var descriptor in descriptors)
            {
                if (descriptor.EventPayload != null &&
                    descriptor.EventPayload is IEvent)
                {
                    events.Add((IEvent)descriptor.EventPayload);
                }
            }
            return events;
        }

        public void Save(IEnumerable<IEvent> events)
        {
            var descriptors = new List<EventDescriptor>();
            descriptors.AddRange(events.Select(e => e.ToDescriptor()));
            this.SaveDescriptors(descriptors);
        }

        public async Task SaveAsync(IEnumerable<IEvent> events, CancellationToken cancellationToken = default(CancellationToken))
        {
            var descriptors = new List<EventDescriptor>();
            descriptors.AddRange(events.Select(e => e.ToDescriptor()));
            await this.SaveDescriptorsAsync(descriptors, cancellationToken);
        }

        protected abstract IEnumerable<EventDescriptor> LoadDescriptors<TKey>(string originatorClrType, TKey originatorId, long sequenceMin, long sequenceMax)
            where TKey : IEquatable<TKey>;

        protected virtual Task<IEnumerable<EventDescriptor>> LoadDescriptorsAsync<TKey>(string originatorClrType, TKey originatorId, long sequenceMin, long sequenceMax, CancellationToken cancellationToken = default(CancellationToken))
            where TKey : IEquatable<TKey> => Task.FromResult(LoadDescriptors(originatorClrType, originatorId, sequenceMin, sequenceMax));

        protected abstract void SaveDescriptors(IEnumerable<EventDescriptor> descriptors);

        protected virtual Task SaveDescriptorsAsync(IEnumerable<EventDescriptor> descriptors, CancellationToken cancellationToken = default(CancellationToken))
            => Task.Factory.StartNew(() => SaveDescriptors(descriptors), cancellationToken);
    }
}
