using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Apworks.Repositories.MongoDB
{
    public sealed class MongoRepositoryContext : RepositoryContext<IMongoClient>
    {
        public MongoRepositoryContext(MongoRepositorySettings settings)
            : base(new MongoClient(settings.ClientSettings))
        {
            this.Settings = settings;
        }

        public MongoRepositoryContext(IMongoClient mongoClient, string databaseName, MongoDatabaseSettings databaseSettings = null, MongoCollectionSettings collectionSettings = null)
            : base(mongoClient)
        {
            this.Settings = new MongoRepositorySettings
            {
                ClientSettings = mongoClient.Settings,
                DatabaseName = databaseName,
                DatabaseSettings = databaseSettings,
                CollectionSettings = collectionSettings
            };
        }

        public MongoRepositorySettings Settings
        {
            get;
        }

        protected override IRepository<TKey, TAggregateRoot> CreateRepository<TKey, TAggregateRoot>()
        {
            return new MongoRepository<TKey, TAggregateRoot>(this);
        }
    }
}
