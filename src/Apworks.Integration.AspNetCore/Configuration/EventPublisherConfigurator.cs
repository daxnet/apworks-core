using Apworks.Events;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;

namespace Apworks.Integration.AspNetCore.Configuration
{
    public interface IEventPublisherConfigurator : IConfigurator
    {
    }

    internal sealed class EventPublisherConfigurator<TEventPublisher> : ServiceRegisterConfigurator<IEventPublisher, TEventPublisher>, IEventPublisherConfigurator
        where TEventPublisher : class, IEventPublisher
    {
        public EventPublisherConfigurator(IConfigurator context, TEventPublisher implementation, ServiceLifetime serviceLifetime) : base(context, implementation, serviceLifetime)
        {
        }

        public EventPublisherConfigurator(IConfigurator context, Func<IServiceProvider, TEventPublisher> implementationFactory, ServiceLifetime serviceLifetime) : base(context, implementationFactory, serviceLifetime)
        {
        }
    }
}
