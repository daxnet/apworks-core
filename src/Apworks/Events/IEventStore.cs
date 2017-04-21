using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Apworks.Events
{
    /// <summary>
    /// Represents that the implemented classes are event stores.
    /// </summary>
    /// <seealso cref="System.IDisposable" />
    public interface IEventStore : IDisposable
    {
        /// <summary>
        /// Saves the specified event descriptors to the current event store.
        /// </summary>
        /// <param name="eventDescriptors">The event descriptors to be saved.</param>
        void Save(IEnumerable<EventDescriptor> eventDescriptors);

        /// <summary>
        /// Saves the specified event descriptors to the current event store asynchronously.
        /// </summary>
        /// <param name="eventDescriptors">The event descriptors to be saved.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task SaveAsync(IEnumerable<EventDescriptor> eventDescriptors, CancellationToken cancellationToken = default(CancellationToken));

        IEnumerable<EventDescriptor> Load<TKey>(string originatorClrType, TKey originatorId)
            where TKey : IEquatable<TKey>;

        Task<IEnumerable<EventDescriptor>> LoadAsync<TKey>(string originatorClrType, TKey originatorId, CancellationToken cancellationToken = default(CancellationToken))
            where TKey : IEquatable<TKey>;
    }
}
