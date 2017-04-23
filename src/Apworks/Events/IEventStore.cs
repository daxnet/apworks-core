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
        /// Saves the specified events to the current event store.
        /// </summary>
        /// <param name="events">The events to be saved.</param>
        void Save(IEnumerable<IEvent> events);

        /// <summary>
        /// Saves the specified events to the current event store asynchronously.
        /// </summary>
        /// <param name="eventDescriptors">The events to be saved.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task SaveAsync(IEnumerable<IEvent> eventDescriptors, CancellationToken cancellationToken = default(CancellationToken));

        IEnumerable<TEvent> Load<TKey, TEvent>(string originatorClrType, TKey originatorId)
            where TKey : IEquatable<TKey>
            where TEvent : IEvent;

        Task<IEnumerable<TEvent>> LoadAsync<TKey, TEvent>(string originatorClrType, TKey originatorId, CancellationToken cancellationToken = default(CancellationToken))
            where TKey : IEquatable<TKey>
            where TEvent : IEvent;
    }
}
