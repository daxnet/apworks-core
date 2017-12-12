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
using Apworks.Serialization.Json;
using Apworks.Integration.AspNetCore.Messaging;
using Microsoft.Extensions.DependencyInjection;
using System.Threading;
using System.Threading.Tasks;

namespace Apworks.Tests
{
    public class SimpleMessageBusTests
    {
        [Fact]
        public void PublishMessageTest()
        {
            var serviceCollection = new ServiceCollection();
            var messageHandlerExecutionContext = new ServiceProviderMessageHandlerExecutionContext(serviceCollection);

            var mb = new SimpleMessageBus(new MessageJsonSerializer(), messageHandlerExecutionContext);
            int numOfMessagesReceived = 0;
            mb.MessageReceived += (x, y) => numOfMessagesReceived++;

            var event1 = new NameChangedEvent("daxnet");
            mb.Publish(event1);

            Assert.Equal(1, numOfMessagesReceived);
        }

        [Fact]
        public void PublishMultipleMessagesTest()
        {
            var serviceCollection = new ServiceCollection();
            var messageHandlerExecutionContext = new ServiceProviderMessageHandlerExecutionContext(serviceCollection);

            var mb = new SimpleMessageBus(new MessageJsonSerializer(), messageHandlerExecutionContext);
            int numOfMessagesReceived = 0;
            mb.MessageReceived += (x, y) => numOfMessagesReceived++;

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
            var serviceCollection = new ServiceCollection();
            var messageHandlerExecutionContext = new ServiceProviderMessageHandlerExecutionContext(serviceCollection);

            var changedName = string.Empty;
            var eventStore = new DictionaryEventStore();
            var eventBus = new SimpleEventBus(new MessageJsonSerializer(), messageHandlerExecutionContext);
            var snapshotProvider = new SuppressedSnapshotProvider();
            eventBus.MessageReceived += (s, e) =>
            {
                if (e.Message is NameChangedEvent)
                {
                    changedName = (e.Message as NameChangedEvent).Name;
                }
            };

            var domainRepository = new EventSourcingDomainRepository(eventStore, eventBus, snapshotProvider);

            var id = Guid.NewGuid();
            var model = new Employee(id);
            model.ChangeName("daxnet");
            domainRepository.Save<Guid, Employee>(model);

            Assert.Equal("daxnet", changedName);
        }

        [Fact]
        public void MessagePublishAndHandleWithEventHandlerTest()
        {
            var serviceCollection = new ServiceCollection();
            var list = new List<string>();
            var messageHandlerExecutionContext = new ServiceProviderMessageHandlerExecutionContext(serviceCollection);
            serviceCollection.AddSingleton(list);

            var eventBus = new SimpleEventBus(new MessageJsonSerializer(), messageHandlerExecutionContext);
            eventBus.Subscribe<NameChangedEvent, NameChangedEventHandler>();

            eventBus.Publish(new NameChangedEvent("myName"));

            Assert.Single(list);
            Assert.Equal("myName", list[0]);
        }

        private class NameChangedEventHandler : Events.EventHandler<NameChangedEvent>
        {
            private readonly List<string> list;

            public NameChangedEventHandler(List<string> list)
            {
                this.list = list;
            }

            public override Task<bool> HandleAsync(NameChangedEvent message, CancellationToken cancellationToken = default(CancellationToken))
            {
                this.list.Add(message.Name);
                return Task.FromResult(true);
            }
        }
    }
}
