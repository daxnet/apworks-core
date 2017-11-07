using Apworks.Events;
using System;
using Microsoft.Extensions.DependencyInjection;

namespace Apworks.Integration.AspNetCore.Configuration
{
    /// <summary>
    /// Represents that the implemented classes are event bus configurators
    /// that configure the event bus in the service collection.
    /// </summary>
    /// <seealso cref="Apworks.Integration.AspNetCore.Configuration.IConfigurator" />
    public interface IEventBusConfigurator : IConfigurator
    {

    }

    /// <summary>
    /// Represents the event bus configurator which configures the event bus in the service collection.
    /// </summary>
    /// <typeparam name="TEventBus">The type of the event bus.</typeparam>
    /// <seealso cref="Apworks.Integration.AspNetCore.Configuration.ServiceRegisterConfigurator{Apworks.Events.IEventBus, TEventBus}" />
    /// <seealso cref="Apworks.Integration.AspNetCore.Configuration.IEventBusConfigurator" />
    internal sealed class EventBusConfigurator<TEventBus> : ServiceRegisterConfigurator<IEventBus, TEventBus>, IEventBusConfigurator
        where TEventBus : class, IEventBus
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EventBusConfigurator{TEventBus}"/> class.
        /// </summary>
        /// <param name="context">The configurator context.</param>
        /// <param name="implementation">The implementation of the <see cref="IEventBus"/> which will be registered in the service collection.</param>
        /// <param name="serviceLifetime">The service lifetime.</param>
        public EventBusConfigurator(IConfigurator context, TEventBus implementation, ServiceLifetime serviceLifetime) 
            : base(context, implementation, serviceLifetime)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EventBusConfigurator{TEventBus}"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="implementationFactory">The implementation factory which creates the event bus and registers the message bus in the service collection.</param>
        /// <param name="serviceLifetime">The service lifetime.</param>
        public EventBusConfigurator(IConfigurator context, Func<IServiceProvider, TEventBus> implementationFactory, ServiceLifetime serviceLifetime) 
            : base(context, implementationFactory, serviceLifetime)
        {
        }
    }
}
