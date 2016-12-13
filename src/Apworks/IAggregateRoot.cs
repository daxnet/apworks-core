using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Apworks
{
    /// <summary>
    /// Represents that the implemented classes are aggregate roots.
    /// </summary>
    /// <typeparam name="TKey">The type of the aggregate root key.</typeparam>
    public interface IAggregateRoot<TKey> : IEntity<TKey>
        where TKey : IEquatable<TKey>
    {
    }
}
