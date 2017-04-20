using Apworks.Events;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using System.Linq;
using Newtonsoft.Json;

namespace Apworks.Tests
{
    public class AggregateRootTests
    {
        [Fact]
        public void ApplyEventTest1()
        {
            var employee = new Employee();
            employee.ChangeName("Sunny");
            Assert.Equal("Sunny", employee.Name);
            Assert.Equal(1, employee.UncommittedEvents.Count());
            Assert.NotEqual(Guid.Empty, employee.UncommittedEvents.First().Id);
            Assert.NotEqual(DateTime.MinValue, employee.UncommittedEvents.First().Timestamp);
        }

        [Fact]
        public void ApplyEventTest2()
        {
            var employee = new Employee();
            employee.ChangeTitle("Software Engineer");
            Assert.Equal("Sr. Software Engineer", employee.Title);
            Assert.Equal(1, employee.UncommittedEvents.Count());
            Assert.NotEqual(Guid.Empty, employee.UncommittedEvents.First().Id);
            Assert.NotEqual(DateTime.MinValue, employee.UncommittedEvents.First().Timestamp);
        }

        [Fact]
        public void ApplyEventTest3()
        {
            var employee = new Employee();
            employee.ChangeName("Sunny");
            employee.ChangeTitle("Software Engineer");
            employee.Register();
            Assert.Equal(3, employee.UncommittedEvents.Count());
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

            var employee = new Employee();
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
            Assert.True(nameChangedEvent.Metadata.ContainsKey(Event.EventClrTypeMetadataKey));
            Assert.True(nameChangedEvent.Metadata.ContainsKey(Event.EventIntentMetadataKey));
        }

        [Fact]
        public void EventMetadataTest2()
        {
            var nameChangedEvent = new NameChangedEvent("daxnet");
            Assert.Equal(typeof(NameChangedEvent).AssemblyQualifiedName, nameChangedEvent.Metadata[Event.EventClrTypeMetadataKey]);
            Assert.Equal(typeof(NameChangedEvent).Name, nameChangedEvent.Metadata[Event.EventIntentMetadataKey]);
        }

        [Fact]
        public void SerializeAndDeserializeEventTest1()
        {
            var eventId = Guid.NewGuid();
            var nameChangedEvent = new NameChangedEvent("daxnet");
            nameChangedEvent.Id = eventId;
            nameChangedEvent.Timestamp = DateTime.UtcNow;
            var json = JsonConvert.SerializeObject(nameChangedEvent);
            Assert.NotNull(json);

            var deserialized = JsonConvert.DeserializeObject<NameChangedEvent>(json);
            Assert.Equal(eventId, deserialized.Id);
            Assert.Equal(nameChangedEvent.Name, deserialized.Name);
            Assert.Equal(nameChangedEvent.Timestamp, deserialized.Timestamp);
        }
    }

    #region Test Data
    class Employee : AggregateRootWithEventSourcing<Guid>
    {
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

    class NameChangedEvent : DomainEvent
    {
        public NameChangedEvent(string name)
        {
            this.Name = name;
        }

        public string Name { get; }
    }

    class TitleChangedEvent : DomainEvent
    {
        public TitleChangedEvent(string title)
        {
            this.Title = title;
        }

        public string Title { get; }
    }

    class RegisteredEvent : DomainEvent
    {

    }
    #endregion
}
