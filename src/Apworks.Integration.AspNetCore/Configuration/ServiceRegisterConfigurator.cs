using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Apworks.Integration.AspNetCore.Configuration
{
    internal abstract class ServiceRegisterConfigurator<TService> : Configurator
        where TService : class
    {
        private readonly TService service;
        private readonly Func<IServiceProvider, TService> serviceFactory;
        private readonly ServiceLifetime serviceLifetime;

        protected ServiceRegisterConfigurator(IConfigurator context, TService service, ServiceLifetime serviceLifetime) : base(context)
        {
            this.service = service;
            this.serviceLifetime = serviceLifetime;
        }

        protected ServiceRegisterConfigurator(IConfigurator context, Func<IServiceProvider, TService> serviceFactory, ServiceLifetime serviceLifetime)
            : base(context)
        {
            this.serviceFactory = serviceFactory;
            this.serviceLifetime = serviceLifetime;
        }

        protected override IServiceCollection DoConfigure(IServiceCollection context)
        {
            if (service != null)
            {
                switch (this.serviceLifetime)
                {
                    case ServiceLifetime.Scoped:
                        return context.AddScoped<TService>(serviceProvider => service);
                    case ServiceLifetime.Singleton:
                        return context.AddSingleton<TService>(serviceProvider => service);
                    default:
                        return context.AddTransient<TService>(serviceProvider => service);
                }
            }

            if (serviceFactory != null)
            {
                switch (this.serviceLifetime)
                {
                    case ServiceLifetime.Scoped:
                        return context.AddScoped<TService>(serviceFactory);
                    case ServiceLifetime.Singleton:
                        return context.AddSingleton<TService>(serviceFactory);
                    default:
                        return context.AddTransient<TService>(serviceFactory);
                }
            }

            return context;
        }
    }
}
