using Apworks.Repositories.MongoDB;
using Apworks.Tests.Integration.Fixtures;
using Apworks.Tests.Integration.Models;
using MongoDB.Driver;
using System;
using System.Linq;
using Xunit;

namespace Apworks.Tests.Integration
{
    public class MongoDBTests : IClassFixture<MongoDBFixture>, IDisposable
    {
        private readonly MongoDBFixture fixture;
        private readonly MongoClient mongoClient;

        public MongoDBTests(MongoDBFixture fixture)
        {
            this.fixture = fixture;
            mongoClient = new MongoClient(fixture.Settings.ClientSettings);
        }

        public void Dispose()
        {
            var database = mongoClient.GetDatabase(fixture.Settings.DatabaseName,
                fixture.Settings.DatabaseSettings);
            var collection = database.GetCollection<Customer>("Customers", fixture.Settings.CollectionSettings);
            collection.DeleteMany(_ => true);
        }

        [Fact]
        public void SaveAggregateRootTest()
        {
            using (var repositoryContext = new MongoRepositoryContext(this.fixture.Settings))
            {
                var customer = Customer.CreateOne(1, "Sunny Chen", "daxnet@abc.com");
                var repository = repositoryContext.GetRepository<int, Customer>();
                repository.Add(customer);
                repositoryContext.Commit();

                var customersCount = repository.FindAll().Count();

                Assert.Equal(1, customersCount);
            }
        }

        [Fact]
        public void Save2AggregateRootsTest()
        {
            using (var repositoryContext = new MongoRepositoryContext(this.fixture.Settings))
            {
                var customers = Customer.CreateMany(2);
                var repository = repositoryContext.GetRepository<int, Customer>();
                customers.ToList().ForEach(customer => repository.Add(customer));
                repositoryContext.Commit();

                var customersCount = repository.FindAll().Count();

                Assert.Equal(2, customersCount);
            }
        }
    }
}
