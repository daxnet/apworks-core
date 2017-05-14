using System;

namespace Apworks.Querying
{
    public interface ISortSpecificationParser
    {
        SortSpecification<TKey, TAggregateRoot> Parse<TKey, TAggregateRoot>(string input)
            where TKey : IEquatable<TKey>
            where TAggregateRoot : class, IAggregateRoot<TKey>;
    }
}
