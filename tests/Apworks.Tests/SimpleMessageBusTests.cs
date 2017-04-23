using Apworks.Messaging;
using Apworks.Messaging.Simple;
using Apworks.Tests.Models;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Xunit;

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
    }
}
