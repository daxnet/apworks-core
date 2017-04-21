using Apworks.Events;
using Apworks.EventStore.Dictionary;
using Apworks.Tests.Models;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Apworks.Tests
{
    public class DictionaryEventStoreTests
    {
        [Fact]
        public void SaveEventsTest1()
        {
            var employee = new Employee();

            var event1 = new NameChangedEvent("daxnet");
            var event2 = new TitleChangedEvent("title");
            var event3 = new RegisteredEvent();

            event1.AttachTo(employee);
            event2.AttachTo(employee);
            event3.AttachTo(employee);

            var store = new DictionaryEventStore();
            store.Save(new List<EventDescriptor>
            {
                event1.ToDescriptor(),
                event2.ToDescriptor(),
                event3.ToDescriptor()
            });


        }
    }
}
