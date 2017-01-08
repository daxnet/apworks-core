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

        public override void Remove(TAggregateRoot aggregateRoot)
        {
            this.collection.DeleteOne(x => x.Id.Equals(aggregateRoot.Id));
        }

        public override async Task RemoveAsync(TAggregateRoot aggregateRoot, CancellationToken cancellationToken = default(CancellationToken))
        {
            await this.collection.DeleteOneAsync(x => x.Id.Equals(aggregateRoot.Id), cancellationToken);
        }

        public override void Update(TAggregateRoot aggregateRoot)
        {
            this.collection.ReplaceOne(x => x.Id.Equals(aggregateRoot.Id), aggregateRoot);
        }

        public override async Task UpdateAsync(TAggregateRoot aggregateRoot, CancellationToken cancellationToken = default(CancellationToken))
        {
            await this.collection.ReplaceOneAsync(x => x.Id.Equals(aggregateRoot.Id), aggregateRoot, cancellationToken: cancellationToken);
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

            List<SortDefinition<TAggregateRoot>> sortDefinitions = new List<SortDefinition<TAggregateRoot>>();
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
    }
}
