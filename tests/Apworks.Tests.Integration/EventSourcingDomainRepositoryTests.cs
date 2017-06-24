using Apworks.Events;
using Apworks.EventStore.AdoNet;
using Apworks.EventStore.PostgreSQL;
using Apworks.KeyGeneration;
using Apworks.Messaging.RabbitMQ;
using Apworks.Repositories;
using Apworks.Serialization.Json;
using Apworks.Tests.Integration.Fixtures;
using Apworks.Tests.Integration.Models;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using Xunit;

namespace Apworks.Tests.Integration
{
    public class EventSourcingDomainRepositoryTests : DisposableObject, IClassFixture<PostgreSQLFixture>
    {
        private static readonly IObjectSerializer serializer =
            new ObjectJsonSerializer(new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All });
        private static readonly IConnectionFactory connectionFactory = new ConnectionFactory { HostName = "localhost" };

        private readonly PostgreSQLFixture fixture;
        private readonly IEventPublisher eventPublisher;
        private readonly IEventStore eventStore;
        private readonly IDomainRepository repository;

        public EventSourcingDomainRepositoryTests(PostgreSQLFixture fixture)
        {
            this.fixture = fixture;
            this.eventPublisher = new EventBus(connectionFactory, serializer, this.GetType().Name);
            this.eventStore = new PostgreSqlEventStore(new AdoNetEventStoreConfiguration(PostgreSQLFixture.ConnectionString, new GuidKeyGenerator()), serializer);
            this.repository = new EventSourcingDomainRepository(this.eventStore, this.eventPublisher);
        }

        [Fact]
        public void SaveAggregateRootTest()
        {
            var aggregateRootId = Guid.NewGuid();
            var employee = new Employee { Id = aggregateRootId };
            employee.ChangeName("daxnet");
            employee.ChangeTitle("developer");
            Assert.Equal(2, employee.Version);
            this.repository.Save<Guid, Employee>(employee);
            Assert.Equal(2, employee.Version);
        }

        [Fact]
        public void LoadAggregateRootTest()
        {
            var aggregateRootId = Guid.NewGuid();
            var employee = new Employee { Id = aggregateRootId };
            employee.ChangeName("daxnet");
            employee.ChangeTitle("developer");
            this.repository.Save<Guid, Employee>(employee);

            var employee2 = this.repository.GetById<Guid, Employee>(aggregateRootId);
            Assert.Equal("daxnet", employee2.Name);
            Assert.Equal("Sr. developer", employee2.Title);
            Assert.Equal(2, employee2.Version);
        }

        [Fact]
        public void SaveAggregateRootAndSubscribeEventTest()
        {
            int eventsReceived = 0;
            var ackCnt = 0;
            var subscriber = (IEventSubscriber)this.eventPublisher;
            subscriber.MessageReceived += (a, b) => eventsReceived++;
            subscriber.MessageAcknowledged += (x, y) => ackCnt++;
            subscriber.Subscribe();

            var aggregateRootId = Guid.NewGuid();
            var employee = new Employee { Id = aggregateRootId };
            employee.ChangeName("daxnet");
            employee.ChangeTitle("developer");
            this.repository.Save<Guid, Employee>(employee);
            while (ackCnt < 2) ;
            Assert.Equal(2, eventsReceived);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.fixture.ClearTable();
                this.repository.Dispose();
            }
        }
    }
}
