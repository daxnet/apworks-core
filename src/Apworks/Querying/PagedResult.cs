using System;
using System.Collections;
using System.Collections.Generic;

namespace Apworks.Querying
{
    /// <summary>
    /// Represents the instance that holds the aggregate roots from a particular
    /// page of the entire aggregate root collection and the pagniation information.
    /// </summary>
    /// <typeparam name="TKey">The type of the key that is used by the aggregate root.</typeparam>
    /// <typeparam name="TAggregateRoot">The type of the aggregate root.</typeparam>
    /// <seealso cref="System.Collections.Generic.ICollection{TAggregateRoot}" />
    public sealed class PagedResult<TKey, TAggregateRoot> : ICollection<TAggregateRoot>
        where TKey : IEquatable<TKey>
        where TAggregateRoot : class, IAggregateRoot<TKey>
    {
        #region Private Fields
        private readonly List<TAggregateRoot> entities = new List<TAggregateRoot>();
        #endregion

        #region Ctor
        public PagedResult(IEnumerable<TAggregateRoot> source, int pageNumber, int pageSize, long totalRecords, long totalPages)
        {
            this.entities.AddRange(source);
            this.PageNumber = pageNumber;
            this.PageSize = pageSize;
            this.TotalRecords = totalRecords;
            this.TotalPages = totalPages;
        }
        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the number of the aggregate roots per page.
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// Gets or sets the page number of the current page.
        /// </summary>
        public int PageNumber { get; set; }

        /// <summary>
        /// Gets or sets the total number of aggregate roots
        /// that are contained in the collection.
        /// </summary>
        public long TotalRecords { get; set; }

        /// <summary>
        /// Gets or sets the total pages of the aggregate roots
        /// that are contained in the collection.
        /// </summary>
        public long TotalPages { get; set; }

        /// <summary>
        /// Gets the number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1" />.
        /// </summary>
        public int Count => entities.Count;

        /// <summary>
        /// Gets a value indicating whether the <see cref="T:System.Collections.Generic.ICollection`1" /> is read-only.
        /// </summary>
        public bool IsReadOnly => false;
        #endregion

        /// <summary>
        /// Adds an item to the <see cref="T:System.Collections.Generic.ICollection`1" />.
        /// </summary>
        /// <param name="item">The object to add to the <see cref="T:System.Collections.Generic.ICollection`1" />.</param>
        public void Add(TAggregateRoot item) => entities.Add(item);

        /// <summary>
        /// 
        /// </summary>
        public void Clear() => entities.Clear();

        public bool Contains(TAggregateRoot item) => entities.Contains(item);

        public void CopyTo(TAggregateRoot[] array, int arrayIndex) => entities.CopyTo(array, arrayIndex);

        public bool Remove(TAggregateRoot item) => entities.Remove(item);

        public IEnumerator<TAggregateRoot> GetEnumerator() => entities.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => entities.GetEnumerator();
    }
}
