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

        public abstract IEnumerable<EventDescriptor> Load<TKey>(string originatorClrType, TKey originatorId) 
            where TKey : IEquatable<TKey>;

        public virtual Task<IEnumerable<EventDescriptor>> LoadAsync<TKey>(string originatorClrType, TKey originatorId, CancellationToken cancellationToken = default(CancellationToken))
            where TKey : IEquatable<TKey> => Task.FromResult(Load<TKey>(originatorClrType, originatorId));

        public abstract void Save(IEnumerable<EventDescriptor> eventDescriptors);

        public virtual Task SaveAsync(IEnumerable<EventDescriptor> eventDescriptors, CancellationToken cancellationToken = default(CancellationToken)) => Task.Factory.StartNew(() => Save(eventDescriptors), cancellationToken);
    }
}
