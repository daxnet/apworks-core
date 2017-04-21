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
        public virtual void Dispose() { }

        public abstract IEnumerable<EventDescriptor> Load<TKey>(string aggregateRootTypeIdentifier, TKey id) where TKey : IEquatable<TKey>;

        public virtual Task<IEnumerable<EventDescriptor>> LoadAsync<TKey>(string aggregateRootTypeIdentifier, TKey id, CancellationToken cancellationToken = default(CancellationToken))
            where TKey : IEquatable<TKey> => Task.FromResult(Load<TKey>(aggregateRootTypeIdentifier, id));

        public abstract void Save(IEnumerable<EventDescriptor> events);

        public virtual Task SaveAsync(IEnumerable<EventDescriptor> events, CancellationToken cancellationToken = default(CancellationToken)) => Task.Factory.StartNew(() => Save(events), cancellationToken);
    }
}
