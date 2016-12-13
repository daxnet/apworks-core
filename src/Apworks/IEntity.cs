using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Apworks
{
    /// <summary>
    /// Represents that the implemented classes are entities.
    /// </summary>
    /// <typeparam name="TKey">The type of the entity key.</typeparam>
    public interface IEntity<TKey>
        where TKey : IEquatable<TKey>
    {
        /// <summary>
        /// Gets or sets the key of the entity.
        /// </summary>
        TKey Id { get; set; }
    }
}
