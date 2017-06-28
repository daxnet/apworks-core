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

        /// <summary>
        /// Loads the specified originator color type.
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TEvent">The type of the event.</typeparam>
        /// <param name="originatorClrType">Type of the originator color.</param>
        /// <param name="originatorId">The originator identifier.</param>
        /// <returns></returns>
        IEnumerable<IEvent> Load<TKey>(string originatorClrType, TKey originatorId, long sequenceMin = EventStore.MinimalSequence, long sequenceMax = EventStore.MaximumSequence)
            where TKey : IEquatable<TKey>;

        Task<IEnumerable<IEvent>> LoadAsync<TKey>(string originatorClrType, TKey originatorId, long sequenceMin = EventStore.MinimalSequence, long sequenceMax = EventStore.MaximumSequence, CancellationToken cancellationToken = default(CancellationToken))
            where TKey : IEquatable<TKey>;
    }
}
