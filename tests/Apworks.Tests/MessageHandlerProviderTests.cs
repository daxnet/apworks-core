using Apworks.Integration.AspNetCore.Messaging;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Apworks.Tests
{
    public class MessageHandlerProviderTests
    {
        [Fact]
        public void RegisterHandlerTest()
        {
            var serviceCollection = new ServiceCollection();
            var provider = new MessageHandlerProvider(serviceCollection);
            provider.RegisterHandler<AggregateUpdatedEvent, AggregateUpdatedEventHandler>();
            Assert.True(provider.HasHandlersRegistered<AggregateUpdatedEvent>());
        }

        [Fact]
        public void ResolveRegisteredHandler()
        {
            var serviceCollection = new ServiceCollection();
            var provider = new MessageHandlerProvider(serviceCollection);
            provider.RegisterHandler<AggregateUpdatedEvent, AggregateUpdatedEventHandler>();
            var handler = provider.GetHandlersFor<AggregateUpdatedEvent>();

            Assert.True(handler.Count() > 0);
        }
    }

    public class AggregateUpdatedEvent : Events.DomainEvent
    {

    }

    public class AggregateUpdatedEventHandler : Events.EventHandler<AggregateUpdatedEvent>
    {
        public override Task<bool> HandleAsync(AggregateUpdatedEvent message, CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }
    }
}
