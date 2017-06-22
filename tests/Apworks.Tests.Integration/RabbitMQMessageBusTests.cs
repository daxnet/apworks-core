using Apworks.Messaging.RabbitMQ;
using Apworks.Serialization.Json;
using Apworks.Tests.Integration.Models;
using Newtonsoft.Json;
using RabbitMQ.Client;
using Xunit;

namespace Apworks.Tests.Integration
{
    public class RabbitMQMessageBusTests
    {
        private readonly IObjectSerializer serializer = new ObjectJsonSerializer(new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All });

        [Fact]
        public void PublishMessageTest()
        {
            int numOfMessagesReceived = 0;
            bool finished = false;

            using (var bus = new MessageBus(new ConnectionFactory() { HostName = "localhost" },
                serializer, 
                "RabbitMQMessageBusTests.PublishMessageTest"))
            {
                // When any message received, increase the counter
                bus.MessageReceived += (x, y) => numOfMessagesReceived++;

                // When any message acknowledged, stop waiting the do the assertion.
                bus.MessageAcknowledged += (x, y) => finished = true;

                bus.Subscribe();

                var event1 = new NameChangedEvent("daxnet");
                bus.Publish(event1);

                while (!finished) ;
            }

            Assert.Equal(1, numOfMessagesReceived);
        }
    }
}
