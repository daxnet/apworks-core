using Apworks.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;

namespace Apworks.Integration.AspNetCore.Configuration
{
    public interface ICommandBusConfigurator : IConfigurator
    {

    }

    internal sealed class CommandBusConfigurator<TCommandBus> : ServiceRegisterConfigurator<ICommandBus, TCommandBus>, ICommandBusConfigurator
        where TCommandBus : class, ICommandBus
    {
        public CommandBusConfigurator(IConfigurator context, TCommandBus implementation, ServiceLifetime serviceLifetime) 
            : base(context, implementation, serviceLifetime)
        {
        }

        public CommandBusConfigurator(IConfigurator context, Func<IServiceProvider, TCommandBus> implementationFactory, ServiceLifetime serviceLifetime) 
            : base(context, implementationFactory, serviceLifetime)
        {
        }
    }
}
