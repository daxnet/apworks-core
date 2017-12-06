using Apworks.Events;
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
        public void ServiceRegisterTest()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddTransient<IEventHandler>(sp => new AggregateUpdatedEventHandler());
            var provider = serviceCollection.BuildServiceProvider();
            Assert.NotNull(provider.GetService<IEventHandler>());
            var svc = provider.GetService<IEventHandler>();
            Assert.IsType<AggregateUpdatedEventHandler>(svc);
        }

        [Fact]
        public void RegisterHandlerTest()
        {
            var serviceCollection = new ServiceCollection();
            var provider = new MessageHandlerProvider(serviceCollection);
            provider.RegisterHandler<AggregateUpdatedEvent, AggregateUpdatedEventHandler>();
            Assert.True(provider.HasHandlersRegistered<AggregateUpdatedEvent>());
        }

        [Fact]
        public void ResolveRegisteredHandlerTest()
        {
            var serviceCollection = new ServiceCollection();
            var provider = new MessageHandlerProvider(serviceCollection);
            provider.RegisterHandler<AggregateUpdatedEvent, AggregateUpdatedEventHandler>();
            var handler = provider.GetHandlersFor<AggregateUpdatedEvent>();
            Assert.True(handler.Count() == 1);
            Assert.IsType<AggregateUpdatedEventHandler>(handler.First());
        }

        [Fact]
        public void ResolveMultipleRegisteredHandlerTest()
        {
            var serviceCollection = new ServiceCollection();
            var provider = new MessageHandlerProvider(serviceCollection);
            provider.RegisterHandler<AggregateUpdatedEvent, AggregateUpdatedEventHandler>();
            provider.RegisterHandler<AggregateUpdatedEvent, AggregateUpdatedEventHandler2>();
            var handler = provider.GetHandlersFor<AggregateUpdatedEvent>();
            Assert.True(handler.Count() == 2);
            Assert.IsType<AggregateUpdatedEventHandler>(handler.First());
            Assert.IsType<AggregateUpdatedEventHandler2>(handler.Last());
        }

        [Fact]
        public void ResolveSameRegisteredHandlerTest()
        {
            var serviceCollection = new ServiceCollection();
            var provider = new MessageHandlerProvider(serviceCollection);
            provider.RegisterHandler<AggregateUpdatedEvent, AggregateUpdatedEventHandler>();

            var handler1 = provider.GetHandlersFor<AggregateUpdatedEvent>().FirstOrDefault();
            var handler2 = provider.GetHandlersFor<AggregateUpdatedEvent>().FirstOrDefault();

            Assert.NotNull(handler1);
            Assert.NotNull(handler2);
            Assert.IsType<AggregateUpdatedEventHandler>(handler1);
            Assert.IsType<AggregateUpdatedEventHandler>(handler2);
            Assert.NotSame(handler1, handler2);
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

    public class AggregateUpdatedEventHandler2 : Events.EventHandler<AggregateUpdatedEvent>
    {
        public override Task<bool> HandleAsync(AggregateUpdatedEvent message, CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }
    }
}
