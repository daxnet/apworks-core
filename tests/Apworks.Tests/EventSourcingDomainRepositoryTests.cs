using Apworks.Events;
using Apworks.EventStore.Simple;
using Apworks.Messaging.Simple;
using Apworks.Repositories;
using Apworks.Snapshots;
using Apworks.Tests.Helpers;
using System;
using System.Linq;
using Xunit;
using Apworks.Serialization.Json;
using Apworks.Messaging;
using Apworks.Integration.AspNetCore.Messaging;
using Microsoft.Extensions.DependencyInjection;

namespace Apworks.Tests
{
    public class EventSourcingDomainRepositoryTests
    {
        private static readonly ServiceCollection serviceCollection = new ServiceCollection();
        private static readonly IMessageHandlerExecutionContext messageHandlerExecutionContext = new ServiceProviderMessageHandlerExecutionContext(serviceCollection);

        private readonly IEventPublisher eventPublisher = new SimpleEventBus(new MessageJsonSerializer(), messageHandlerExecutionContext);
        private readonly IEventStore eventStore = new DictionaryEventStore();
        private readonly IDomainRepository repository;
        private readonly ISnapshotProvider snapshotProvider = new SuppressedSnapshotProvider();

        public EventSourcingDomainRepositoryTests()
        {
            this.repository = new EventSourcingDomainRepository(eventStore, eventPublisher, snapshotProvider);
        }

        [Fact]
        public void SaveAggregateRootTest()
        {
            var aggregateRootId = Guid.NewGuid();
            var employee = new Employee (aggregateRootId);
            employee.ChangeName("daxnet");
            employee.ChangeTitle("developer");
            Assert.Equal(3, employee.Version);
            this.repository.Save<Guid, Employee>(employee);
            Assert.Equal(3, employee.Version);
        }

        [Fact]
        public void LoadAggregateRootTest()
        {
            var aggregateRootId = Guid.NewGuid();
            var employee = new Employee(aggregateRootId);
            employee.ChangeName("daxnet");
            employee.ChangeTitle("developer");
            this.repository.Save<Guid, Employee>(employee);

            var employee2 = this.repository.GetById<Guid, Employee>(aggregateRootId);
            Assert.Equal(aggregateRootId, employee2.Id);
            Assert.Equal("daxnet", employee2.Name);
            Assert.Equal("Sr. developer", employee2.Title);
            Assert.Equal(3, employee2.Version);
        }

        [Fact]
        public void EventSequenceAfterSaveTest()
        {
            var aggregateRootId = Guid.NewGuid();
            var employee = new Employee(aggregateRootId);
            employee.ChangeName("daxnet");
            employee.ChangeTitle("developer");
            this.repository.Save<Guid, Employee>(employee);

            var events = this.eventStore.Load<Guid>(typeof(Employee).AssemblyQualifiedName, aggregateRootId).ToList();
            Assert.Equal(3, events.Count);
            Assert.Equal(1, (events[0] as IDomainEvent).Sequence);
            Assert.Equal(2, (events[1] as IDomainEvent).Sequence);
            Assert.Equal(3, (events[2] as IDomainEvent).Sequence);
        }

        [Fact]
        public void SaveSnapshotCountTest1()
        {
            var aggregateRootId = Guid.NewGuid();
            var employee = new Employee (aggregateRootId);
            var localSnapshotProvider = new InMemorySnapshotProvider();
            var localRepository = new EventSourcingDomainRepository(eventStore, eventPublisher, localSnapshotProvider);
            for (var i = 0; i < 31; i++)
            {
                employee.ChangeName($"daxnet_{i}");
            }
            localRepository.Save<Guid, Employee>(employee);
            Assert.Single(localSnapshotProvider.snapshots);
        }

        [Fact]
        public void SaveSnapshotCountTest2()
        {
            var aggregateRootId = Guid.NewGuid();
            var employee = new Employee (aggregateRootId);
            var localSnapshotProvider = new InMemorySnapshotProvider();
            var localRepository = new EventSourcingDomainRepository(eventStore, eventPublisher, localSnapshotProvider);
            for (var i = 0; i < 31; i++)
            {
                employee.ChangeName($"daxnet_{i}");
            }
            localRepository.Save<Guid, Employee>(employee);

            for (var i = 0; i < 31; i++)
            {
                employee.ChangeTitle($"Developer_{i}");
            }
            localRepository.Save<Guid, Employee>(employee);

            Assert.Equal(2, localSnapshotProvider.snapshots.Count);
        }

        [Fact]
        public void SaveSnapshotCountTest3()
        {
            var aggregateRootId = Guid.NewGuid();
            var employee = new Employee (aggregateRootId);
            var localSnapshotProvider = new InMemorySnapshotProvider();
            var localRepository = new EventSourcingDomainRepository(eventStore, eventPublisher, localSnapshotProvider);
            for (var i = 0; i < 31; i++)
            {
                employee.ChangeName($"daxnet_{i}");
            }
            localRepository.Save<Guid, Employee>(employee);

            for (var i = 0; i < 15; i++)
            {
                employee.ChangeTitle($"Developer_{i}");
            }
            localRepository.Save<Guid, Employee>(employee);

            Assert.Single(localSnapshotProvider.snapshots);
        }

        [Fact]
        public void LoadFromSnapshotTest1()
        {
            var aggregateRootId = Guid.NewGuid();
            var employee = new Employee (aggregateRootId);
            var localSnapshotProvider = new InMemorySnapshotProvider();
            var localRepository = new EventSourcingDomainRepository(eventStore, eventPublisher, localSnapshotProvider);
            for (var i = 0; i < 31; i++)
            {
                employee.ChangeName($"daxnet_{i}");
            }
            localRepository.Save<Guid, Employee>(employee);

            var employee2 = localRepository.GetById<Guid, Employee>(aggregateRootId);
            Assert.Equal(employee.Name, employee2.Name);
        }

        [Fact]
        public void LoadFromSnapshotTest2()
        {
            var aggregateRootId = Guid.NewGuid();
            var employee = new Employee(aggregateRootId);
            var localSnapshotProvider = new InMemorySnapshotProvider();
            var localRepository = new EventSourcingDomainRepository(eventStore, eventPublisher, localSnapshotProvider);
            for (var i = 0; i < 31; i++)
            {
                employee.ChangeName($"daxnet_{i}");
            }
            localRepository.Save<Guid, Employee>(employee);

            for (var i = 0; i < 4; i++)
            {
                employee.ChangeTitle($"Software Developer_{i}");
            }
            localRepository.Save<Guid, Employee>(employee);

            var employee2 = localRepository.GetById<Guid, Employee>(aggregateRootId);
            Assert.Equal(employee.Name, employee2.Name);
            Assert.Equal(employee.Title, employee2.Title);
        }
    }
}
