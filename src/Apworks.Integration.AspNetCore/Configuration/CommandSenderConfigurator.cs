using Apworks.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;

namespace Apworks.Integration.AspNetCore.Configuration
{
    public interface ICommandSenderConfigurator : IConfigurator
    {
    }

    internal sealed class CommandSenderConfigurator<TCommandSender> : ServiceRegisterConfigurator<ICommandSender, TCommandSender>, ICommandSenderConfigurator
        where TCommandSender : class, ICommandSender
    {
        public CommandSenderConfigurator(IConfigurator context, TCommandSender implementation, ServiceLifetime serviceLifetime) : base(context, implementation, serviceLifetime)
        {
        }

        public CommandSenderConfigurator(IConfigurator context, Func<IServiceProvider, TCommandSender> implementationFactory, ServiceLifetime serviceLifetime) : base(context, implementationFactory, serviceLifetime)
        {
        }


    }
}
