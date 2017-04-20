using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Apworks.Events
{
    public abstract class DomainEventStore : IDomainEventStore
    {
        public virtual void Dispose() { }

        public abstract IEnumerable<IDomainEvent> Load<TKey>(string aggregateRootTypeIdentifier, TKey id) where TKey : IEquatable<TKey>;

        public virtual Task<IEnumerable<IDomainEvent>> LoadAsync<TKey>(string aggregateRootTypeIdentifier, TKey id, CancellationToken cancellationToken = default(CancellationToken))
            where TKey : IEquatable<TKey> => Task.FromResult(Load<TKey>(aggregateRootTypeIdentifier, id));

        public abstract void Save(IEnumerable<IDomainEvent> events);

        public virtual Task SaveAsync(IEnumerable<IDomainEvent> events, CancellationToken cancellationToken = default(CancellationToken)) => Task.Factory.StartNew(() => Save(events), cancellationToken);
    }
}
