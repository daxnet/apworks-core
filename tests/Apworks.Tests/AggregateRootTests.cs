using Apworks.Events;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using System.Linq;

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
        }

        [Fact]
        public void ApplyEventTest2()
        {
            var employee = new Employee();
            employee.ChangeTitle("Software Engineer");
            Assert.Equal("Sr. Software Engineer", employee.Title);
            Assert.Equal(1, employee.UncommittedEvents.Count());
        }
    }

    #region Test Data
    class Employee : AggregateRoot<Guid>
    {
        public string Name { get; private set; }

        public string Title { get; private set; }

        public void ChangeName(string name)
        {
            this.Apply<NameChangedEvent>(new NameChangedEvent(name));
        }

        public void ChangeTitle(string title)
        {
            this.Apply<TitleChangedEvent>(new TitleChangedEvent(title));
        }

        [Handles(typeof(TitleChangedEvent))]
        private void HandlesTitleChangedEvent(TitleChangedEvent evnt)
        {
            this.Title = $"Sr. {evnt.Title}";
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
    #endregion
}
