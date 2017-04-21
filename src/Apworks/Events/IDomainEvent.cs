using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apworks.Events
{
    public interface IDomainEvent : IEvent
    {
        /// <summary>
        /// Attaches the current domain event to the specified aggregate root.
        /// </summary>
        /// <typeparam name="TKey">The type of the aggregate root key.</typeparam>
        /// <param name="aggregateRoot">The aggregate root to which the current event will be attached.</param>
        void AttachTo<TKey>(IAggregateRoot<TKey> aggregateRoot)
            where TKey : IEquatable<TKey>;
    }
}
