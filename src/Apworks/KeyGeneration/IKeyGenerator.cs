using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Apworks.KeyGeneration
{
    public interface IKeyGenerator<out TKey, in TAggregateRoot>
        where TKey : IEquatable<TKey>
        where TAggregateRoot : class, IAggregateRoot<TKey>
    {
        TKey Generate(TAggregateRoot aggregateRoot);
    }
}
