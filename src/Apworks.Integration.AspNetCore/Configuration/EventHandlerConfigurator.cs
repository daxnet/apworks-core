using Apworks.Events;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;

namespace Apworks.Integration.AspNetCore.Configuration
{
    public interface IEventHandlerConfigurator : IConfigurator
    {

    }

    internal sealed class EventHandlerConfigurator<TEventHandler> : ServiceRegisterConfigurator<IEventHandler, TEventHandler>, IEventHandlerConfigurator
        where TEventHandler : class, IEventHandler
    {
        public EventHandlerConfigurator(IConfigurator context, TEventHandler implementation, ServiceLifetime serviceLifetime) 
            : base(context, implementation, serviceLifetime)
        {
        }

        public EventHandlerConfigurator(IConfigurator context, Func<IServiceProvider, TEventHandler> implementationFactory, ServiceLifetime serviceLifetime) 
            : base(context, implementationFactory, serviceLifetime)
        {
        }
    }
}
