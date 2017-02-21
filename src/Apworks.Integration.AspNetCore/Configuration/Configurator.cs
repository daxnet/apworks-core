using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Apworks.Integration.AspNetCore.Configuration
{
    public interface IConfigurator
    {
        IServiceCollection Configure();
    }

    internal abstract class Configurator : IConfigurator
    {
        private readonly IConfigurator context;

        protected Configurator(IConfigurator context)
        {
            this.context = context;
        }

        public IServiceCollection Configure()
        {
            var serviceCollection = this.context.Configure();
            return DoConfigure(serviceCollection);
        }

        protected abstract IServiceCollection DoConfigure(IServiceCollection context);
    }
}
