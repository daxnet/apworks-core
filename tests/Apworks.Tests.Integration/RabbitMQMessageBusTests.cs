using Apworks.Events;
using Apworks.Integration.AspNetCore.Messaging;
using Apworks.Messaging;
using Apworks.Messaging.RabbitMQ;
using Apworks.Serialization.Json;
using Apworks.Tests.Integration.Models;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Apworks.Tests.Integration
{
    public class RabbitMQMessageBusTests
    {
        private readonly IMessageSerializer serializer = new MessageJsonSerializer();
        private readonly ConnectionFactory connectionFactory = new ConnectionFactory() { HostName = "localhost" };

        //[Fact]
        //public void PublishMessageTest()
        //{
        //    int numOfMessagesReceived = 0;
        //    bool finished = false;

        //    using (var bus = new RabbitMessageBus(connectionFactory,
        //        serializer, 
        //        "RabbitMQMessageBusTests.PublishMessageTest", queueName: "RabbitMQMessageBusTests.PublishMessageTestQueue"))
        //    {
        //        // When any message received, increase the counter
        //        bus.MessageReceived += (x, y) => numOfMessagesReceived++;

        //        // When any message acknowledged, stop waiting the do the assertion.
        //        bus.MessageAcknowledged += (x, y) => finished = true;

        //        bus.Subscribe();

        //        var event1 = new NameChangedEvent("daxnet");
        //        bus.Publish(event1);

        //        while (!finished) ;
        //    }

        //    Assert.Equal(1, numOfMessagesReceived);
        //}

        /*
        [Fact]
        public async Task SimpleEventHandlerTest()
        {
            bool finished = false;
            var serviceCollection = new ServiceCollection();
            var names = new List<string>();
            serviceCollection.AddTransient(x => names);

            var messageHandlerProvider = new ServiceProviderMessageHandlerExecutionContext(serviceCollection);
            messageHandlerProvider.RegisterHandler<NameChangedEvent, NameChangedEventHandler>();

            using (var eventPublisher = new RabbitEventBus(connectionFactory, serializer, "RabbitMQMessageBusTests.SimpleEventHandlerTest"))
            using (var eventSubscriber = new RabbitEventBus(connectionFactory, serializer, "RabbitMQMessageBusTests.SimpleEventHandlerTest"))
            using (var eventConsumer = new EventConsumer(eventSubscriber, messageHandlerProvider))
            {
                eventSubscriber.MessageAcknowledged += (x, y) => finished = true;
                eventConsumer.Consume();

                await eventPublisher.PublishAsync(new NameChangedEvent("daxnet"));
                while (!finished) ;

                Assert.Equal(1, names.Count);
                Assert.Equal("daxnet", names[0]);
            }
        }

        [Fact]
        public async Task MultipleEventHandlersTest()
        {
            bool finished = false;
            var serviceCollection = new ServiceCollection();
            var names = new List<string>();
            serviceCollection.AddTransient(x => names);

            var messageHandlerProvider = new ServiceProviderMessageHandlerExecutionContext(serviceCollection);
            messageHandlerProvider.RegisterHandler<NameChangedEvent, NameChangedEventHandler>();
            messageHandlerProvider.RegisterHandler<NameChangedEvent, NameChangedEventHandler2>();

            using (var eventPublisher = new RabbitEventBus(connectionFactory, serializer, "RabbitMQMessageBusTests.SimpleEventHandlerTest"))
            using (var eventSubscriber = new RabbitEventBus(connectionFactory, serializer, "RabbitMQMessageBusTests.SimpleEventHandlerTest"))
            using (var eventConsumer = new EventConsumer(eventSubscriber, messageHandlerProvider))
            {
                eventSubscriber.MessageAcknowledged += (x, y) => finished = true;
                eventConsumer.Consume();

                await eventPublisher.PublishAsync(new NameChangedEvent("daxnet"));
                while (!finished) ;

                Assert.Equal(2, names.Count);
                Assert.Equal("daxnet", names[0]);
                Assert.Equal("daxnet2", names[1]);
            }
        }
        */
    }
}
