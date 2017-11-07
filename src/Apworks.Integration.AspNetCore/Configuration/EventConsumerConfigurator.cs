using Apworks.Events;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;

namespace Apworks.Integration.AspNetCore.Configuration
{
    public interface IEventConsumerConfigurator : IConfigurator
    {

    }


    internal class EventConsumerConfigurator<TEventConsumer> : ServiceRegisterConfigurator<IEventConsumer, TEventConsumer>, IEventConsumerConfigurator
        where TEventConsumer : class, IEventConsumer
    {
        public EventConsumerConfigurator(IConfigurator context, TEventConsumer implementation, ServiceLifetime serviceLifetime) : base(context, implementation, serviceLifetime)
        {
        }

        public EventConsumerConfigurator(IConfigurator context, Func<IServiceProvider, TEventConsumer> implementationFactory, ServiceLifetime serviceLifetime) : base(context, implementationFactory, serviceLifetime)
        {
        }
        
    }

    internal sealed class EventConsumerConfigurator : EventConsumerConfigurator<EventConsumer>
    {
        public EventConsumerConfigurator(IConfigurator context, ServiceLifetime serviceLifetime, string route = null)
            : base(context, x => new EventConsumer(x.GetService<IEventSubscriber>(), x.GetServices<IEventHandler>(), route), serviceLifetime)
        {
        }
    }

}
