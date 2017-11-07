using Apworks.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;

namespace Apworks.Integration.AspNetCore.Configuration
{
    public interface ICommandConsumerConfigurator : IConfigurator
    { }

    internal class CommandConsumerConfigurator<TCommandConsumer> : ServiceRegisterConfigurator<ICommandConsumer, TCommandConsumer>, ICommandConsumerConfigurator
        where TCommandConsumer : class, ICommandConsumer
    {
        public CommandConsumerConfigurator(IConfigurator context, TCommandConsumer implementation, ServiceLifetime serviceLifetime) : base(context, implementation, serviceLifetime)
        {
        }

        public CommandConsumerConfigurator(IConfigurator context, Func<IServiceProvider, TCommandConsumer> implementationFactory, ServiceLifetime serviceLifetime) : base(context, implementationFactory, serviceLifetime)
        {
        }
    }

    internal sealed class CommandConsumerConfigurator : CommandConsumerConfigurator<CommandConsumer>
    {
        public CommandConsumerConfigurator(IConfigurator context, ServiceLifetime serviceLifetime, string route = null)
            : base(context, x => new CommandConsumer(x.GetService<ICommandSubscriber>(), x.GetServices<ICommandHandler>(), route), serviceLifetime)
        { }
    }
}
