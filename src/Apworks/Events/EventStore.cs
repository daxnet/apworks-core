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
        private readonly IObjectSerializer payloadSerializer;

        protected EventStore(IObjectSerializer payloadSerializer)
        {
            this.payloadSerializer = payloadSerializer;
        }

        protected IObjectSerializer PayloadSerializer { get => this.payloadSerializer; }

        public virtual void Dispose() { }

        public IEnumerable<IEvent> Load<TKey>(string originatorClrType, TKey originatorId) 
            where TKey : IEquatable<TKey>
        {
            var descriptors = this.LoadDescriptors<TKey>(originatorClrType, originatorId);
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

        public async Task<IEnumerable<IEvent>> LoadAsync<TKey>(string originatorClrType, TKey originatorId, CancellationToken cancellationToken = default(CancellationToken))
            where TKey : IEquatable<TKey>
        {
            var descriptors = await this.LoadDescriptorsAsync<TKey>(originatorClrType, originatorId, cancellationToken);
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

        protected abstract IEnumerable<EventDescriptor> LoadDescriptors<TKey>(string originatorClrType, TKey originatorId)
            where TKey : IEquatable<TKey>;

        protected virtual Task<IEnumerable<EventDescriptor>> LoadDescriptorsAsync<TKey>(string originatorClrType, TKey originatorId, CancellationToken cancellationToken = default(CancellationToken))
            where TKey : IEquatable<TKey> => Task.FromResult(LoadDescriptors(originatorClrType, originatorId));

        protected abstract void SaveDescriptors(IEnumerable<EventDescriptor> descriptors);

        protected virtual Task SaveDescriptorsAsync(IEnumerable<EventDescriptor> descriptors, CancellationToken cancellationToken = default(CancellationToken))
            => Task.Factory.StartNew(() => SaveDescriptors(descriptors), cancellationToken);
    }
}
