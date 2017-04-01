using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Apworks.Querying;
using Microsoft.EntityFrameworkCore;

namespace Apworks.Repositories.EntityFramework
{
    internal sealed class EntityFrameworkRepository<TKey, TAggregateRoot> : Repository<TKey, TAggregateRoot>
        where TKey : IEquatable<TKey>
        where TAggregateRoot : class, IAggregateRoot<TKey>
    {
        private const string IdPropertyName = "Id";
        private readonly DbContext dbContext;

        public EntityFrameworkRepository(IRepositoryContext context) : base(context)
        {
            var entityFrameworkRepositoryContext = (EntityFrameworkRepositoryContext)context;
            this.dbContext = entityFrameworkRepositoryContext.Session;
        }

        public override void Add(TAggregateRoot aggregateRoot)
        {
            this.dbContext
                .Set<TAggregateRoot>()
                .Add(aggregateRoot);
        }

        public override async Task AddAsync(TAggregateRoot aggregateRoot, CancellationToken cancellationToken = default(CancellationToken))
        {
            await this.dbContext
                .Set<TAggregateRoot>()
                .AddAsync(aggregateRoot, cancellationToken);
        }

        public override IEnumerable<TAggregateRoot> FindAll(Expression<Func<TAggregateRoot, bool>> specification, SortSpecification<TKey, TAggregateRoot> sortSpecification)
        {
            var query = this.dbContext.Set<TAggregateRoot>().Where(specification);
            if (sortSpecification?.Count > 0)
            {
                IOrderedQueryable<TAggregateRoot> orderedQuery = null;
                (from sort in sortSpecification.Specifications
                 where sort.Item2 != SortOrder.Unspecified
                 select sort)
                .ToList()
                .ForEach(sort =>
                {
                    switch (sort.Item2)
                    {
                        case SortOrder.Ascending:
                            orderedQuery = orderedQuery == null ? query.OrderBy(sort.Item1) : orderedQuery.OrderBy(sort.Item1);
                            break;
                        case SortOrder.Descending:
                            orderedQuery = orderedQuery == null ? query.OrderByDescending(sort.Item1) : orderedQuery.OrderByDescending(sort.Item1);
                            break;
                    }
                });

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

            var query = this.dbContext.Set<TAggregateRoot>().Where(specification);
            var total = query.Count();
            if (total == 0)
            {
                return PagedResult<TKey, TAggregateRoot>.CreateDefault(pageNumber, pageSize);
            }

            var skip = (pageNumber - 1) * pageSize;
            var take = pageSize;

            IOrderedQueryable<TAggregateRoot> orderedQuery = null;
            foreach(var sort in sorts)
            {
                switch(sort.Item2)
                {
                    case SortOrder.Ascending:
                        orderedQuery = orderedQuery == null ? query.OrderBy(sort.Item1) : orderedQuery.OrderBy(sort.Item1);
                        break;
                    case SortOrder.Descending:
                        orderedQuery = orderedQuery == null ? query.OrderByDescending(sort.Item1) : orderedQuery.OrderByDescending(sort.Item1);
                        break;
                }
            }

            //var pagedQuery = orderedQuery.Skip(skip).Take(take).GroupBy(p => new { Total = total }).FirstOrDefault();
            //return pagedQuery == null ? null :
            //    new PagedResult<TKey, TAggregateRoot>(pagedQuery.Select(_ => _), pageNumber, pageSize, pagedQuery.Key.Total, (pagedQuery.Key.Total + pageSize - 1) / pageSize);
            var pagedQuery = orderedQuery.Skip(skip).Take(take);
            return pagedQuery == null ? null :
                new PagedResult<TKey, TAggregateRoot>(pagedQuery, pageNumber, pageSize, total, (total + pageSize - 1) / pageSize);
        }

        public override TAggregateRoot FindByKey(TKey key)
        {
            return this.dbContext.Find<TAggregateRoot>(key);
        }

        public override async Task<TAggregateRoot> FindByKeyAsync(TKey key, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await this.dbContext.FindAsync<TAggregateRoot>(key);
        }

        public override void RemoveByKey(TKey key)
        {
            var aggregateRoot = this.FindByKey(key);
            if (aggregateRoot != null)
            {
                this.dbContext.Set<TAggregateRoot>().Remove(aggregateRoot);
            }
        }

        public override async Task RemoveByKeyAsync(TKey key, CancellationToken cancellationToken = default(CancellationToken))
        {
            var aggregateRoot = await this.FindByKeyAsync(key, cancellationToken);
            if (aggregateRoot != null)
            {
                this.dbContext.Set<TAggregateRoot>().Remove(aggregateRoot);
            }
        }

        public override void UpdateByKey(TKey key, TAggregateRoot aggregateRoot)
        {
            this.dbContext.Set<TAggregateRoot>().Update(aggregateRoot);
        }

        private static Expression<Func<TAggregateRoot, bool>> BuildIdEqualsPredicate(TKey id) =>
            Expression.Lambda<Func<TAggregateRoot, bool>>(Expression.Equal(Expression.Property(Expression.Parameter(typeof(TAggregateRoot)), IdPropertyName), Expression.Constant(id)));
    }
}
