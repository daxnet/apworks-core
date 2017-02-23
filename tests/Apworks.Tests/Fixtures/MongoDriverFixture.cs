using Apworks.Tests.Models;
using MongoDB.Driver;
using Moq;

namespace Apworks.Tests.Fixtures
{
    /// <summary>
    /// Represents the test fixture that initializes the context for the MongoDB C# driver.
    /// </summary>
    public class MongoDriverFixture
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MongoDriverFixture"/> class.
        /// </summary>
        public MongoDriverFixture()
        {
            MongoClient = new Mock<IMongoClient>();
            MongoDatabase = new Mock<IMongoDatabase>();
            MongoCollection = new Mock<IMongoCollection<Customer>>();

            MongoDatabase.Setup(x => x.GetCollection<Customer>(It.IsAny<string>(), It.IsAny<MongoCollectionSettings>())).Returns(this.MongoCollection.Object);
            MongoClient.Setup(x => x.GetDatabase(It.IsAny<string>(), It.IsAny<MongoDatabaseSettings>())).Returns(this.MongoDatabase.Object);
        }

        /// <summary>
        /// Gets the mocked mongo client.
        /// </summary>
        /// <value>
        /// The mongo client.
        /// </value>
        public Mock<IMongoClient> MongoClient { get; }

        /// <summary>
        /// Gets the mocked mongo database.
        /// </summary>
        /// <value>
        /// The mongo database.
        /// </value>
        public Mock<IMongoDatabase> MongoDatabase { get; }

        /// <summary>
        /// Gets the mocked mongo collection.
        /// </summary>
        /// <value>
        /// The mongo collection.
        /// </value>
        public Mock<IMongoCollection<Customer>> MongoCollection { get; }
    }
}
