using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Apworks.Repositories.MongoDB
{
    public class MongoRepositoryContext : RepositoryContext<MongoClient>
    {
        public MongoRepositoryContext(MongoRepositorySettings settings)
            :base(new MongoClient(settings.ClientSettings))
        {
            this.Settings = settings;
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
