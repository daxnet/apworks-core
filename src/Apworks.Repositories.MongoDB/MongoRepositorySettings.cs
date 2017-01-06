using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Apworks.Repositories.MongoDB
{
    public sealed class MongoRepositorySettings
    {
        private const int DefaultMongoPortNumber = 27017;

        public MongoRepositorySettings()
        {

        }

        public MongoRepositorySettings(string host, string databaseName)
            : this(host, DefaultMongoPortNumber, databaseName)
        { }

        public MongoRepositorySettings(string host, int port, string databaseName)
        {
            this.ClientSettings = new MongoClientSettings
            {
                Server = new MongoServerAddress(host, port)
            };

            this.DatabaseName = databaseName;
        }

        public MongoClientSettings ClientSettings { get; set; }

        public MongoDatabaseSettings DatabaseSettings { get; set; }

        public MongoCollectionSettings CollectionSettings { get; set; }

        public string DatabaseName { get; set; }
    }
}
