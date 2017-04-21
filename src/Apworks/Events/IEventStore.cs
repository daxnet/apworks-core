using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Apworks.Events
{
    public interface IEventStore : IDisposable
    {
        void Save(IEnumerable<EventDescriptor> eventDescriptors);

        Task SaveAsync(IEnumerable<EventDescriptor> events, CancellationToken cancellationToken = default(CancellationToken));

        IEnumerable<EventDescriptor> Load<TKey>(string aggregateRootTypeIdentifier, TKey id)
            where TKey : IEquatable<TKey>;

        Task<IEnumerable<EventDescriptor>> LoadAsync<TKey>(string aggregateRootTypeIdentifier, TKey id, CancellationToken cancellationToken = default(CancellationToken))
            where TKey : IEquatable<TKey>;
    }
}
