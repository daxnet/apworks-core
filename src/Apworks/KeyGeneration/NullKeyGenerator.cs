using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Apworks.KeyGeneration
{
    public sealed class NullKeyGenerator<TKey> : IKeyGenerator<TKey, IAggregateRoot<TKey>>
        where TKey : IEquatable<TKey>
    {
        public TKey Generate(IAggregateRoot<TKey> aggregateRoot)
        {
            return default(TKey);
        }
    }
}
