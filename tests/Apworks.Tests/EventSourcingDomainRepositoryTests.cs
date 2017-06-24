using Apworks.Events;
using Apworks.EventStore.Simple;
using Apworks.Messaging.Simple;
using Apworks.Repositories;
using Apworks.Tests.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace Apworks.Tests
{
    public class EventSourcingDomainRepositoryTests
    {
        private readonly IEventPublisher eventPublisher = new EventBus();
        private readonly IEventStore eventStore = new DictionaryEventStore();
        private readonly IDomainRepository repository;

        public EventSourcingDomainRepositoryTests()
        {
            this.repository = new EventSourcingDomainRepository(eventStore, eventPublisher);
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
        public void EventSequenceAfterSaveTest()
        {
            var aggregateRootId = Guid.NewGuid();
            var employee = new Employee { Id = aggregateRootId };
            employee.ChangeName("daxnet");
            employee.ChangeTitle("developer");
            this.repository.Save<Guid, Employee>(employee);

            var events = this.eventStore.Load<Guid>(typeof(Employee).AssemblyQualifiedName, aggregateRootId).ToList();
            Assert.Equal(2, events.Count);
            Assert.Equal(1, (events[0] as IDomainEvent).Sequence);
            Assert.Equal(2, (events[1] as IDomainEvent).Sequence);
        }
    }
}
