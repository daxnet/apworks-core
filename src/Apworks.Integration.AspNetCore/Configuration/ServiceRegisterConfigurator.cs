using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Apworks.Integration.AspNetCore.Configuration
{
    internal abstract class ServiceRegisterConfigurator<TService, TImplementation> : Configurator
        where TService : class
        where TImplementation : class, TService
    {
        private readonly TImplementation implementation;
        private readonly Func<IServiceProvider, TImplementation> implementationFactory;
        private readonly ServiceLifetime serviceLifetime;

        protected ServiceRegisterConfigurator(IConfigurator context, TImplementation implementation, ServiceLifetime serviceLifetime) : base(context)
        {
            this.implementation = implementation;
            this.serviceLifetime = serviceLifetime;
        }

        protected ServiceRegisterConfigurator(IConfigurator context, Func<IServiceProvider, TImplementation> implementationFactory, ServiceLifetime serviceLifetime)
            : base(context)
        {
            this.implementationFactory = implementationFactory;
            this.serviceLifetime = serviceLifetime;
        }

        protected override IServiceCollection DoConfigure(IServiceCollection context)
        {
            if (implementation != null)
            {
                switch (this.serviceLifetime)
                {
                    case ServiceLifetime.Scoped:
                        context.AddScoped<TService, TImplementation>(serviceProvider => implementation);
                        break;
                    case ServiceLifetime.Singleton:
                        context.AddSingleton<TService, TImplementation>(serviceProvider => implementation);
                        break;
                    default:
                        context.AddTransient<TService, TImplementation>(serviceProvider => implementation);
                        break;
                }
            }

            if (implementationFactory != null)
            {
                switch (this.serviceLifetime)
                {
                    case ServiceLifetime.Scoped:
                        context.AddScoped<TService, TImplementation>(implementationFactory);
                        break;
                    case ServiceLifetime.Singleton:
                        context.AddSingleton<TService, TImplementation>(implementationFactory);
                        break;
                    default:
                        context.AddTransient<TService, TImplementation>(implementationFactory);
                        break;
                }
            }

            return context;
        }
    }
}
