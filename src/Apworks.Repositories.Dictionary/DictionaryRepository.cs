using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Apworks.Querying;

namespace Apworks.Repositories.Dictionary
{
    internal sealed class DictionaryRepository<TKey, TAggregateRoot> : Repository<TKey, TAggregateRoot>
        where TKey : IEquatable<TKey>
        where TAggregateRoot : class, IAggregateRoot<TKey>
    {
        private readonly DictionaryRepositoryContext context;

        /// <summary>
        /// Initializes a new instance of the <see cref="DictionaryRepository{TKey, TAggregateRoot}"/> class.
        /// </summary>
        /// <param name="context">The repository context.</param>
        public DictionaryRepository(IRepositoryContext context) : base(context)
        {
            this.context = (DictionaryRepositoryContext)context;
        }

        /// <summary>
        /// Adds the specified <see cref="T:Apworks.IAggregateRoot`1" /> instance to the current repository.
        /// </summary>
        /// <param name="aggregateRoot">The <see cref="T:Apworks.IAggregateRoot`1" /> instance to be added.</param>
        public override void Add(TAggregateRoot aggregateRoot)
        {
            this.context.Session.TryAdd(aggregateRoot.Id, aggregateRoot);
        }

        /// <summary>
        /// Gets all the <see cref="T:Apworks.IAggregateRoot`1" /> instances from current repository according to a given query specification with the sorting enabled.
        /// </summary>
        /// <param name="specification">The specification which specifies the query criteria.</param>
        /// <param name="sortSpecification">The specifications which implies the sorting.</param>
        /// <returns>
        /// A <see cref="T:System.Linq.IEnumerable`1" /> instance which queries over the collection of the <see cref="T:Apworks.IAggregateRoot`1" /> objects.
        /// </returns>
        public override IEnumerable<TAggregateRoot> FindAll(Expression<Func<TAggregateRoot, bool>> specification, SortSpecification<TKey, TAggregateRoot> sortSpecification)
        {
            if (specification == null)
            {
                specification = _ => true;
            }

            var query = this.context.Session.Values.Select(x => (TAggregateRoot)x).Where(specification.Compile());
            if (sortSpecification?.Count > 0)
            {
                IOrderedEnumerable<TAggregateRoot> orderedQuery = null;
                foreach(var sort in sortSpecification.Specifications)
                {
                    if (orderedQuery == null)
                    {
                        orderedQuery = sort.Item2 == SortOrder.Descending ? query.OrderByDescending(sort.Item1.Compile()) : query.OrderBy(sort.Item1.Compile());
                    }
                    else
                    {
                        orderedQuery = sort.Item2 == SortOrder.Descending ? orderedQuery.OrderByDescending(sort.Item1.Compile()) : orderedQuery.OrderBy(sort.Item1.Compile());
                    }
                }
                return orderedQuery;
            }

            return query;
        }

        public override PagedResult<TKey, TAggregateRoot> FindAll(Expression<Func<TAggregateRoot, bool>> specification, SortSpecification<TKey, TAggregateRoot> sortSpecification, int pageNumber, int pageSize)
        {
            if (specification == null)
            {
                specification = _ => true;
            }

            if (sortSpecification?.Count == 0)
            {
                throw new ArgumentNullException(nameof(sortSpecification), "The sort specification has not been specified.");
            }

            if (pageNumber <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(pageNumber), "The page number should be greater than 0.");
            }

            if (pageSize <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(pageSize), "The page size should be greater than 0.");
            }

            var sorts = sortSpecification.Specifications.ToList();
            if (sorts.Any(s => s.Item2 == SortOrder.Unspecified))
            {
                throw new InvalidOperationException("The SortOrder of the items in the sort specification should be either Ascending or Descending.");
            }

            var query = this.context.Session.Values.Select(x => (TAggregateRoot)x).Where(specification.Compile());

            var totalCount = query.Count();
            var skip = (pageNumber - 1) * pageSize;
            var take = pageSize;
            var totalPages = (totalCount + pageSize - 1) / pageSize;
            
            IOrderedEnumerable<TAggregateRoot> orderedQuery = null;
            foreach (var sort in sorts)
            {
                if (orderedQuery == null)
                {
                    orderedQuery = sort.Item2 == SortOrder.Descending ? query.OrderByDescending(sort.Item1.Compile()) : query.OrderBy(sort.Item1.Compile());
                }
                else
                {
                    orderedQuery = sort.Item2 == SortOrder.Descending ? orderedQuery.OrderByDescending(sort.Item1.Compile()) : orderedQuery.OrderBy(sort.Item1.Compile());
                }
            }

            return new PagedResult<TKey, TAggregateRoot>(orderedQuery.Skip(skip).Take(take), pageNumber, pageSize, totalCount, totalPages);
        }

        public override TAggregateRoot FindByKey(TKey key)
        {
            object result = null;
            if (this.context.Session.TryGetValue(key, out result))
            {
                return (TAggregateRoot)result;
            }

            return null;
        }

        public override void RemoveByKey(TKey key)
        {
            object result = null;
            this.context.Session.TryRemove(key, out result);
        }

        public override void UpdateByKey(TKey key, TAggregateRoot aggregateRoot)
        {
            var comparison = this.FindByKey(key);
            if (comparison != null)
            {
                aggregateRoot.Id = key;
                this.context.Session.TryUpdate(key, aggregateRoot, comparison);
            }
        }
    }
}
