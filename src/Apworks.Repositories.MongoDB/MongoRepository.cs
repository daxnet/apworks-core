using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Apworks.Repositories.MongoDB
{
    public class MongoRepository<TKey, TAggregateRoot> : Repository<TKey, TAggregateRoot>
        where TKey : IEquatable<TKey>
        where TAggregateRoot : IAggregateRoot<TKey>
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
