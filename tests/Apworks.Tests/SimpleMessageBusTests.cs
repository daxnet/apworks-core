using Apworks.Messaging;
using Apworks.Messaging.Simple;
using Apworks.Tests.Models;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Apworks.EventStore.Simple;
using Apworks.Repositories;
using Apworks.Snapshots;

namespace Apworks.Tests
{
    public class SimpleMessageBusTests
    {
        [Fact]
        public void PublishMessageTest()
        {
            var mb = new MessageBus();
            int numOfMessagesReceived = 0;
            mb.MessageReceived += (x, y) => numOfMessagesReceived++;
            mb.Subscribe();
            mb.Subscribe(); // Test subscribe multiple times.

            var event1 = new NameChangedEvent("daxnet");
            mb.Publish(event1);

            Assert.Equal(1, numOfMessagesReceived);
        }

        [Fact]
        public void PublishMultipleMessagesTest()
        {
            var mb = new MessageBus();
            int numOfMessagesReceived = 0;
            mb.MessageReceived += (x, y) => numOfMessagesReceived++;
            mb.Subscribe();

            var events = new List<IMessage>
            {
                new NameChangedEvent("daxnet"), new TitleChangedEvent("SE"), new RegisteredEvent()
            };

            mb.PublishAll(events);

            Assert.Equal(3, numOfMessagesReceived);
        }

        [Fact]
        public void SimpleCQRSScenarioTest1()
        {
            var changedName = string.Empty;
            var eventStore = new DictionaryEventStore();
            var eventBus = new EventBus();
            var snapshotProvider = new SuppressedSnapshotProvider();
            eventBus.MessageReceived += (s, e) =>
            {
                if (e.Message is NameChangedEvent)
                {
                    changedName = (e.Message as NameChangedEvent).Name;
                }
            };
            eventBus.Subscribe();
            var domainRepository = new EventSourcingDomainRepository(eventStore, eventBus, snapshotProvider);

            var id = Guid.NewGuid();
            var model = new Employee(id);
            model.ChangeName("daxnet");
            domainRepository.Save<Guid, Employee>(model);

            Assert.Equal("daxnet", changedName);
        }
    }
}
