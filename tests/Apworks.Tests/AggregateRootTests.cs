using Apworks.Events;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using System.Linq;
using Newtonsoft.Json;
using Apworks.Tests.Models;

namespace Apworks.Tests
{
    public class AggregateRootTests
    {
        [Fact]
        public void ApplyEventTest1()
        {
            var employee = new Employee(Guid.NewGuid());
            employee.ChangeName("Sunny");
            Assert.Equal("Sunny", employee.Name);
            Assert.Equal(2, employee.UncommittedEvents.Count());
            Assert.NotEqual(Guid.Empty, employee.UncommittedEvents.First().Id);
            Assert.NotEqual(DateTime.MinValue, employee.UncommittedEvents.First().Timestamp);
        }

        [Fact]
        public void ApplyEventTest2()
        {
            var employee = new Employee(Guid.NewGuid());
            employee.ChangeTitle("Software Engineer");
            Assert.Equal("Sr. Software Engineer", employee.Title);
            Assert.Equal(2, employee.UncommittedEvents.Count());
            Assert.NotEqual(Guid.Empty, employee.UncommittedEvents.First().Id);
            Assert.NotEqual(DateTime.MinValue, employee.UncommittedEvents.First().Timestamp);
            Assert.NotNull(employee.UncommittedEvents.First().GetOriginatorClrType());
            Assert.NotNull(employee.UncommittedEvents.First().GetOriginatorIdentifier());
        }

        [Fact]
        public void ApplyEventTest3()
        {
            var employee = new Employee(Guid.NewGuid());
            employee.ChangeName("Sunny");
            employee.ChangeTitle("Software Engineer");
            employee.Register();
            Assert.Equal(4, employee.UncommittedEvents.Count());
        }

        [Fact]
        public void EventSequenceTest()
        {
            var employee = new Employee(Guid.NewGuid());
            employee.ChangeName("Sunny");
            employee.ChangeTitle("Software Engineer");
            employee.Register();

            var events = employee.UncommittedEvents.ToList();
            Assert.Equal(1, events[0].Sequence);
            Assert.Equal(2, events[1].Sequence);
            Assert.Equal(3, events[2].Sequence);
            Assert.Equal(4, events[3].Sequence);
        }

        [Fact]
        public void ReplayEventsTest()
        {
            var events = new List<IDomainEvent>
            {
                new NameChangedEvent("daxnet"),
                new TitleChangedEvent("racer"),
                new RegisteredEvent()
            };

            var employee = new Employee(Guid.NewGuid());
            employee.Replay(events);

            Assert.Equal(0, employee.UncommittedEvents.Count());
            Assert.Equal("daxnet", employee.Name);
            Assert.Equal("Sr. racer", employee.Title);
            Assert.NotEqual(DateTime.MinValue, employee.DateRegistered);
        }

        [Fact]
        public void EventMetadataTest1()
        {
            var nameChangedEvent = new NameChangedEvent("daxnet");
            Assert.Equal(2, nameChangedEvent.Metadata.Count);
        }

        [Fact]
        public void EventMetadataTest2()
        {
            var nameChangedEvent = new NameChangedEvent("daxnet");
            Assert.Equal(typeof(NameChangedEvent).AssemblyQualifiedName, nameChangedEvent.GetMessageClrType());
            Assert.Equal(typeof(NameChangedEvent).Name, nameChangedEvent.GetEventIntent());
        }

        [Fact]
        public void EventMetadataTest3()
        {
            var nameChangedEvent = new NameChangedEvent("daxnet");
            Assert.Null(nameChangedEvent.GetOriginatorClrType());
            Assert.Null(nameChangedEvent.GetOriginatorIdentifier());
        }

        [Fact]
        public void SerializeAndDeserializeEventTest1()
        {
            var nameChangedEvent = new NameChangedEvent("daxnet");
            var eventId = nameChangedEvent.Id;

            nameChangedEvent.Timestamp = DateTime.UtcNow;
            var json = JsonConvert.SerializeObject(nameChangedEvent);
            Assert.NotNull(json);

            var deserialized = JsonConvert.DeserializeObject<NameChangedEvent>(json);
            Assert.Equal(eventId, deserialized.Id);
            Assert.Equal(nameChangedEvent.Name, deserialized.Name);
            Assert.Equal(nameChangedEvent.Timestamp, deserialized.Timestamp);
        }

        [Fact]
        public void ReplayEventsVersionTest()
        {
            var events = new List<IDomainEvent>
            {
                new NameChangedEvent("daxnet"),
                new TitleChangedEvent("racer"),
                new RegisteredEvent()
            };

            var employee = new Employee(Guid.NewGuid());
            employee.Replay(events);

            Assert.Equal(4, employee.Version);
        }
    }

    #region Test Data
    class Employee : AggregateRootWithEventSourcing<Guid>
    {
        public Employee(Guid id)
            : base(id)
        { }

        public string Name { get; private set; }

        public string Title { get; private set; }

        public DateTime DateRegistered { get; private set; }

        public void ChangeName(string name)
        {
            this.Apply<NameChangedEvent>(new NameChangedEvent(name));
        }

        public void ChangeTitle(string title)
        {
            this.Apply<TitleChangedEvent>(new TitleChangedEvent(title));
        }

        public void Register()
        {
            this.Apply<RegisteredEvent>();
        }

        [Handles(typeof(TitleChangedEvent))]
        private void HandlesTitleChangedEvent(TitleChangedEvent evnt)
        {
            this.Title = $"Sr. {evnt.Title}";
        }

        [Handles(typeof(RegisteredEvent))]
        private void HandlesRegisteredEvent(RegisteredEvent evnt)
        {
            this.DateRegistered = DateTime.UtcNow;
        }
    }

    
    #endregion
}
