using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Apworks.Repositories.MongoDB
{
    public sealed class MongoRepositorySettings
    {
        public MongoClientSettings ClientSettings { get; set; }

        public MongoDatabaseSettings DatabaseSettings { get; set; }

        public MongoCollectionSettings CollectionSettings { get; set; }

        public string DatabaseName { get; set; }
    }
}
