using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Apworks.Integration.AspNetCore.Configuration
{
    public interface IApworksConfigurator : IConfigurator
    {

    }

    internal sealed class ApworksConfigurator : IApworksConfigurator
    {
        private readonly IServiceCollection serviceCollection;

        public ApworksConfigurator(IServiceCollection serviceCollection)
        {
            this.serviceCollection = serviceCollection;
        }

        public IServiceCollection Configure() => this.serviceCollection;
    }
}
