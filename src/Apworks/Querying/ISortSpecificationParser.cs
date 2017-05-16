using System;

namespace Apworks.Querying
{
    /// <summary>
    /// Represents that the implemented classes are sort specification parsers
    /// that can parse an input string into the <see cref="SortSpecification{TKey, TAggregateRoot}"/>
    /// instance which will be used as the definition of sorting in a query.
    /// </summary>
    public interface ISortSpecificationParser
    {
        /// <summary>
        /// Parses the given <see cref="string"/> value into <see cref="SortSpecification{TKey, TAggregateRoot}"/>.
        /// </summary>
        /// <typeparam name="TKey">The type of the aggregate root key.</typeparam>
        /// <typeparam name="TAggregateRoot">The type of the aggregate root.</typeparam>
        /// <param name="input">The string value to be parsed as the sort specification.</param>
        /// <returns>The sort specification.</returns>
        SortSpecification<TKey, TAggregateRoot> Parse<TKey, TAggregateRoot>(string input)
            where TKey : IEquatable<TKey>
            where TAggregateRoot : class, IAggregateRoot<TKey>;
    }
}
