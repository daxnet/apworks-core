using Apworks.Events;
using Apworks.EventStore.AdoNet;
using Apworks.EventStore.PostgreSQL;
using Apworks.KeyGeneration;
using Apworks.Messaging;
using Apworks.Messaging.RabbitMQ;
using Apworks.Repositories;
using Apworks.Serialization.Json;
using Apworks.Snapshots;
using Apworks.Tests.Integration.Fixtures;
using Apworks.Tests.Integration.Models;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Linq;
using System.Threading;
using Xunit;

namespace Apworks.Tests.Integration
{
    
    public class EventSourcingDomainRepositoryTests : DisposableObject, IClassFixture<PostgreSQLFixture>
    {
        private static readonly IMessageSerializer messageSerializer = new MessageJsonSerializer();
        private static readonly IObjectSerializer eventStoreSerializer = new ObjectJsonSerializer();
        private static readonly IConnectionFactory connectionFactory = new ConnectionFactory { HostName = "localhost" };

        private readonly PostgreSQLFixture fixture;
        private readonly ISnapshotProvider snapshotProvider = new SuppressedSnapshotProvider();

        public EventSourcingDomainRepositoryTests(PostgreSQLFixture fixture)
        {
            Monitor.Enter(PostgreSQLFixture.locker);
            // Thread.Sleep(1000);
            this.fixture = fixture;
        }

        [Fact]
        public void SaveAggregateRootTest()
        {
            using (var eventPublisher = new EventBus(connectionFactory, messageSerializer, this.GetType().Name))
            using (var eventStore = new PostgreSqlEventStore(new AdoNetEventStoreConfiguration(PostgreSQLFixture.ConnectionString, new GuidKeyGenerator()), eventStoreSerializer))
            using (var repository = new EventSourcingDomainRepository(eventStore, eventPublisher, snapshotProvider))
            {
                var aggregateRootId = Guid.NewGuid();
                var employee = new Employee(aggregateRootId);
                employee.ChangeName("daxnet");
                employee.ChangeTitle("developer");
                Assert.Equal(3, employee.Version);
                repository.Save<Guid, Employee>(employee);
                Assert.Equal(3, employee.Version);
            }
        }

        [Fact]
        public void LoadAggregateRootTest()
        {
            using (var eventPublisher = new EventBus(connectionFactory, messageSerializer, this.GetType().Name))
            using (var eventStore = new PostgreSqlEventStore(new AdoNetEventStoreConfiguration(PostgreSQLFixture.ConnectionString, new GuidKeyGenerator()), eventStoreSerializer))
            using (var repository = new EventSourcingDomainRepository(eventStore, eventPublisher, snapshotProvider))
            {
                var aggregateRootId = Guid.NewGuid();
                var employee = new Employee(aggregateRootId);
                employee.ChangeName("daxnet");
                employee.ChangeTitle("developer");
                repository.Save<Guid, Employee>(employee);

                var employee2 = repository.GetById<Guid, Employee>(aggregateRootId);
                Assert.Equal("daxnet", employee2.Name);
                Assert.Equal("Sr. developer", employee2.Title);
                Assert.Equal(3, employee2.Version);
            }
        }

        [Fact]
        public void SaveAggregateRootAndSubscribeEventTest()
        {
            using (var eventPublisher = new EventBus(connectionFactory, messageSerializer, this.GetType().Name))
            using (var eventStore = new PostgreSqlEventStore(new AdoNetEventStoreConfiguration(PostgreSQLFixture.ConnectionString, new GuidKeyGenerator()), eventStoreSerializer))
            using (var repository = new EventSourcingDomainRepository(eventStore, eventPublisher, snapshotProvider))
            {
                int eventsReceived = 0;
                var ackCnt = 0;
                var subscriber = (IEventSubscriber)eventPublisher;
                subscriber.MessageReceived += (a, b) => eventsReceived++;
                subscriber.MessageAcknowledged += (x, y) => ackCnt++;
                subscriber.Subscribe();

                var aggregateRootId = Guid.NewGuid();
                var employee = new Employee(aggregateRootId);
                employee.ChangeName("daxnet");
                employee.ChangeTitle("developer");
                repository.Save<Guid, Employee>(employee);
                while (ackCnt < 2) ;
                Assert.Equal(2, eventsReceived);
            }
        }

        [Fact]
        public void EventSequenceAfterSaveTest()
        {
            using (var eventPublisher = new EventBus(connectionFactory, messageSerializer, this.GetType().Name))
            using (var eventStore = new PostgreSqlEventStore(new AdoNetEventStoreConfiguration(PostgreSQLFixture.ConnectionString, new GuidKeyGenerator()), eventStoreSerializer))
            using (var repository = new EventSourcingDomainRepository(eventStore, eventPublisher, snapshotProvider))
            {
                var aggregateRootId = Guid.NewGuid();
                var employee = new Employee(aggregateRootId);
                employee.ChangeName("daxnet");
                employee.ChangeTitle("developer");
                repository.Save<Guid, Employee>(employee);

                var events = eventStore.Load<Guid>(typeof(Employee).AssemblyQualifiedName, aggregateRootId).ToList();
                Assert.Equal(3, events.Count);
                Assert.Equal(1, (events[0] as IDomainEvent).Sequence);
                Assert.Equal(2, (events[1] as IDomainEvent).Sequence);
                Assert.Equal(3, (events[2] as IDomainEvent).Sequence);
            }
        }

        [Fact]
        public void GetByVersionTest1()
        {
            using (var eventPublisher = new EventBus(connectionFactory, messageSerializer, this.GetType().Name))
            using (var eventStore = new PostgreSqlEventStore(new AdoNetEventStoreConfiguration(PostgreSQLFixture.ConnectionString, new GuidKeyGenerator()), eventStoreSerializer))
            using (var repository = new EventSourcingDomainRepository(eventStore, eventPublisher, snapshotProvider))
            {
                var aggregateRootId = Guid.NewGuid();
                var employee = new Employee(aggregateRootId);
                employee.ChangeName("daxnet");
                employee.ChangeTitle("developer");
                employee.Register();
                repository.Save<Guid, Employee>(employee);

                var employee2 = repository.GetById<Guid, Employee>(aggregateRootId, 2);  // Load 2 events as the first one is the aggregate created event.
                Assert.Equal(employee.Name, employee2.Name);
                Assert.Null(employee2.Title);
                Assert.Equal(DateTime.MinValue, employee2.DateRegistered);
            }
        }

        [Fact]
        public void GetByVersionTest2()
        {
            using (var eventPublisher = new EventBus(connectionFactory, messageSerializer, this.GetType().Name))
            using (var eventStore = new PostgreSqlEventStore(new AdoNetEventStoreConfiguration(PostgreSQLFixture.ConnectionString, new GuidKeyGenerator()), eventStoreSerializer))
            using (var repository = new EventSourcingDomainRepository(eventStore, eventPublisher, snapshotProvider))
            {
                var aggregateRootId = Guid.NewGuid();
                var employee = new Employee(aggregateRootId);
                employee.ChangeName("daxnet");
                employee.ChangeTitle("developer");
                employee.Register();
                repository.Save<Guid, Employee>(employee);

                var employee2 = repository.GetById<Guid, Employee>(aggregateRootId, 3);
                Assert.Equal(employee.Name, employee2.Name);
                Assert.Equal(employee.Title, employee2.Title);
                Assert.Equal(DateTime.MinValue, employee2.DateRegistered);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.fixture.ClearTables();
            }
            //Thread.Sleep(1000);
            Monitor.Exit(PostgreSQLFixture.locker);
        }
    }
}
