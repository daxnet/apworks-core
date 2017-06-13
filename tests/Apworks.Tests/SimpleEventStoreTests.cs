using Apworks.Events;
using Apworks.EventStore.Simple;
using Apworks.Tests.Models;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using System.Linq;

namespace Apworks.Tests
{
    public class SimpleEventStoreTests
    {
        [Fact]
        public void SaveEventsTest1()
        {
            var aggregateRootId = Guid.NewGuid();
            var employee = new Employee { Id = aggregateRootId };

            var event1 = new NameChangedEvent("daxnet");
            var event2 = new TitleChangedEvent("title");
            var event3 = new RegisteredEvent();

            event1.AttachTo(employee);
            event2.AttachTo(employee);
            event3.AttachTo(employee);

            var store = new DictionaryEventStore();
            store.Save(new List<DomainEvent>
            {
                event1,
                event2,
                event3
            });

            var events = store.Load<Guid>(typeof(Employee).AssemblyQualifiedName, aggregateRootId);
            Assert.Equal(3, events.Count());
            Assert.NotEqual(Guid.Empty, event1.Id);
            Assert.NotEqual(Guid.Empty, event2.Id);
            Assert.NotEqual(Guid.Empty, event3.Id);
        }
    }
}
