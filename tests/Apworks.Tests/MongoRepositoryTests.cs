using Apworks.Repositories.MongoDB;
using Apworks.Tests.Fixtures;
using Apworks.Tests.Models;
using MongoDB.Driver;
using Moq;
using System.Collections.Generic;
using System.Threading;
using Xunit;

namespace Apworks.Tests
{
    public class MongoRepositoryTests : IClassFixture<MongoDriverFixture>
    {
        private readonly MongoDriverFixture fixture;

        public MongoRepositoryTests(MongoDriverFixture fixture)
        {
            this.fixture = fixture;
        }

        [Fact]
        public void AddTest()
        {
            // Arrange
            var cache = new List<Customer>();
            this.fixture.MongoCollection.Reset();
            this.fixture.MongoCollection.Setup(x => x.InsertOne(It.IsAny<Customer>(),
                It.IsAny<InsertOneOptions>(), It.IsAny<CancellationToken>()))
                    .Callback<Customer, InsertOneOptions, CancellationToken>((t, u, v) => cache.Add(t));
            // Act
            var mockMongoRepositoryContext = new MongoRepositoryContext(this.fixture.MongoClient.Object, "mockDatabase");
            var mongoRepository = mockMongoRepositoryContext.GetRepository<int, Customer>();
            mongoRepository.Add(new Customer { Id = 10 });

            // Assert
            Assert.Single(cache);
            Assert.Equal(10, cache[0].Id);
        }
    }
}
