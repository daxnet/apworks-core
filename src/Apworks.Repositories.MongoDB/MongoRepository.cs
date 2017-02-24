using MongoDB.Driver;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Apworks.Querying;
using System.Linq.Expressions;
using System.Collections.Generic;

namespace Apworks.Repositories.MongoDB
{
    /// <summary>
    /// Represents the MongoDB implementation of the repository.
    /// </summary>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <typeparam name="TAggregateRoot">The type of the aggregate root.</typeparam>
    /// <seealso cref="Apworks.Repositories.Repository{TKey, TAggregateRoot}" />
    public class MongoRepository<TKey, TAggregateRoot> : Repository<TKey, TAggregateRoot>
        where TKey : IEquatable<TKey>
        where TAggregateRoot : class, IAggregateRoot<TKey>
    {
        private readonly IMongoCollection<TAggregateRoot> collection;

        public MongoRepository(IRepositoryContext context) : base(context)
        {
            var mongoContext = (MongoRepositoryContext)context;

            var database = mongoContext.Session.GetDatabase(mongoContext.Settings.DatabaseName, 
                mongoContext.Settings.DatabaseSettings);

            var collectionName = typeof(TAggregateRoot).Name.Pluralize();
            this.collection = database.GetCollection<TAggregateRoot>(collectionName,
                mongoContext.Settings.CollectionSettings);
        }

        public override void Add(TAggregateRoot aggregateRoot)
        {
            var options = new InsertOneOptions { BypassDocumentValidation = true };
            this.collection.InsertOne(aggregateRoot, options);
        }

        public override async Task AddAsync(TAggregateRoot aggregateRoot, CancellationToken cancellationToken = default(CancellationToken))
        {
            var options = new InsertOneOptions { BypassDocumentValidation = true };
            await this.collection.InsertOneAsync(aggregateRoot, options, cancellationToken);
        }

        public override void RemoveByKey(TKey key)
        {
            var filterDefinition = Builders<TAggregateRoot>.Filter.Eq(x => x.Id, key);
            this.collection.DeleteOne(filterDefinition);
        }

        public override async Task RemoveByKeyAsync(TKey key, CancellationToken cancellationToken = default(CancellationToken))
        {
            var filterDefinition = Builders<TAggregateRoot>.Filter.Eq(x => x.Id, key);
            await this.collection.DeleteOneAsync(filterDefinition, cancellationToken);
        }

        public override void UpdateByKey(TKey key, TAggregateRoot aggregateRoot)
        {
            var filterDefinition = Builders<TAggregateRoot>.Filter.Eq(x => x.Id, key);
            this.collection.ReplaceOne(filterDefinition, aggregateRoot);
        }

        public override async Task UpdateByKeyAsync(TKey key, TAggregateRoot aggregateRoot, CancellationToken cancellationToken = default(CancellationToken))
        {
            var filterDefinition = Builders<TAggregateRoot>.Filter.Eq(x => x.Id, key);
            await this.collection.ReplaceOneAsync(filterDefinition, aggregateRoot, cancellationToken: cancellationToken);
        }

        public override IQueryable<TAggregateRoot> FindAll(Expression<Func<TAggregateRoot, bool>> specification, SortSpecification<TKey, TAggregateRoot> sortSpecification)
        {
            var find = this.collection.Find(specification);
            if (sortSpecification == null)
            {
                return find.ToEnumerable().AsQueryable();
            }

            var sorts = sortSpecification.Specifications.ToList();
            if (sorts.Any(s => s.Item2 == SortOrder.Unspecified))
            {
                return find.ToEnumerable().AsQueryable();
            }

            var firstSort = sorts[0];
            IOrderedFindFluent<TAggregateRoot, TAggregateRoot> orderedFind = firstSort.Item2 == SortOrder.Ascending ?
                find.SortBy(firstSort.Item1) : find.SortByDescending(firstSort.Item1);

            for (var i = 1; i < sorts.Count; i++)
            {
                var current = sorts[i];
                orderedFind = current.Item2 == SortOrder.Ascending ?
                    orderedFind.SortBy(current.Item1) : orderedFind.SortByDescending(current.Item1);
            }

            return orderedFind.ToEnumerable().AsQueryable();
        }

        public override async Task<IQueryable<TAggregateRoot>> FindAllAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            return await this.FindAllAsync(_ => true, null, cancellationToken);
        }

        public override async Task<IQueryable<TAggregateRoot>> FindAllAsync(Expression<Func<TAggregateRoot, bool>> specification, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await this.FindAllAsync(specification, null, cancellationToken);
        }

        public override async Task<IQueryable<TAggregateRoot>> FindAllAsync(Expression<Func<TAggregateRoot, bool>> specification, SortSpecification<TKey, TAggregateRoot> sortSpecification, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (sortSpecification == null || sortSpecification.Specifications == null || sortSpecification.Specifications.Count() == 0)
            {
                return (await this.collection.FindAsync(specification, cancellationToken: cancellationToken))
                    .ToEnumerable(cancellationToken)
                    .AsQueryable();
            }

            var sorts = sortSpecification.Specifications.ToList();
            if (sorts.Any(s => s.Item2 == SortOrder.Unspecified))
            {
                return (await this.collection.FindAsync(specification, cancellationToken: cancellationToken))
                    .ToEnumerable(cancellationToken)
                    .AsQueryable();
            }

            var sortDefinitions = new List<SortDefinition<TAggregateRoot>>();
            foreach(var sort in sorts)
            {
                sortDefinitions.Add(sort.Item2 == SortOrder.Ascending ?
                    Builders<TAggregateRoot>.Sort.Ascending(sort.Item1) :
                    Builders<TAggregateRoot>.Sort.Descending(sort.Item1));
            }

            var aggregatedSortDefinition = Builders<TAggregateRoot>.Sort.Combine(sortDefinitions);
            var findOptions = new FindOptions<TAggregateRoot> { Sort = aggregatedSortDefinition };
            return (await this.collection.FindAsync(specification, findOptions, cancellationToken))
                    .ToEnumerable(cancellationToken)
                    .AsQueryable();
        }

        public override TAggregateRoot FindByKey(TKey key)
        {
            return this.collection.Find(x => x.Id.Equals(key)).FirstOrDefault();
        }

        public override async Task<TAggregateRoot> FindByKeyAsync(TKey key, CancellationToken cancellationToken = default(CancellationToken))
        {
            return (await this.collection.FindAsync(x => x.Id.Equals(key), cancellationToken: cancellationToken)).FirstOrDefault();
        }

        public override PagedResult<TKey, TAggregateRoot> FindAll(Expression<Func<TAggregateRoot, bool>> specification, SortSpecification<TKey, TAggregateRoot> sortSpecification, int pageNumber, int pageSize)
        {
            if (specification == null)
            {
                specification = _ => true;
            }

            if (sortSpecification == null)
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

            var totalCount = this.collection.Count(specification);
            var skip = (pageNumber - 1) * pageSize;
            var take = pageSize;
            var totalPages = (totalCount + pageSize - 1) / pageSize;

            var find = this.collection.Find(specification);
            var firstSort = sorts[0];
            var orderedFind = firstSort.Item2 == SortOrder.Ascending ?
                find.SortBy(firstSort.Item1) : find.SortByDescending(firstSort.Item1);

            for (var i = 1; i < sorts.Count; i++)
            {
                var current = sorts[i];
                orderedFind = current.Item2 == SortOrder.Ascending ?
                    orderedFind.SortBy(current.Item1) : orderedFind.SortByDescending(current.Item1);
            }

            return new PagedResult<TKey, TAggregateRoot>(find.Skip(skip).Limit(take).ToEnumerable(), pageNumber, pageSize, totalCount, totalPages);
        }

        public override async Task<PagedResult<TKey, TAggregateRoot>> FindAllAsync(Expression<Func<TAggregateRoot, bool>> specification, SortSpecification<TKey, TAggregateRoot> sortSpecification, int pageNumber, int pageSize, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (specification == null)
            {
                specification = _ => true;
            }

            if (sortSpecification == null)
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

            var sortDefinitions = new List<SortDefinition<TAggregateRoot>>();
            foreach (var sort in sortSpecification.Specifications)
            {
                sortDefinitions.Add(sort.Item2 == SortOrder.Ascending ?
                    Builders<TAggregateRoot>.Sort.Ascending(sort.Item1) :
                    Builders<TAggregateRoot>.Sort.Descending(sort.Item1));
            }

            var totalCount = await this.collection.CountAsync(specification);
            var skip = (pageNumber - 1) * pageSize;
            var take = pageSize;
            var totalPages = (totalCount + pageSize - 1) / pageSize;

            var aggregatedSortDefinition = Builders<TAggregateRoot>.Sort.Combine(sortDefinitions);
            var findOptions = new FindOptions<TAggregateRoot> { Sort = aggregatedSortDefinition, Skip = skip, Limit = take };

            return new PagedResult<TKey, TAggregateRoot>((await this.collection.FindAsync<TAggregateRoot>(specification, findOptions)).ToEnumerable(),
                pageNumber, pageSize, totalCount, totalPages);
        }
    }
}
