using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Apworks.Events
{
    public interface IDomainEventStore : IDisposable
    {
        void Save(IEnumerable<IDomainEvent> events);

        Task SaveAsync(IEnumerable<IDomainEvent> events, CancellationToken cancellationToken = default(CancellationToken));

        IEnumerable<IDomainEvent> Load<TKey>(string aggregateRootTypeIdentifier, TKey id)
            where TKey : IEquatable<TKey>;

        Task<IEnumerable<IDomainEvent>> LoadAsync<TKey>(string aggregateRootTypeIdentifier, TKey id, CancellationToken cancellationToken = default(CancellationToken))
            where TKey : IEquatable<TKey>;
    }
}
