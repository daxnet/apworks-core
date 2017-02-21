using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Apworks.Repositories;

namespace Apworks.Integration.AspNetCore.Configuration
{
    public interface IRepositoryConfigurator : IConfigurator
    {
    }

    internal sealed class RepositoryConfigurator<TRepositoryContext> : ServiceRegisterConfigurator<TRepositoryContext>, IRepositoryConfigurator
        where TRepositoryContext : class, IRepositoryContext
    {
        public RepositoryConfigurator(IConfigurator context, TRepositoryContext repositoryContext, ServiceLifetime serviceLifetime)
            : base(context, repositoryContext, serviceLifetime)
        { }

        public RepositoryConfigurator(IConfigurator context, Func<IServiceProvider, TRepositoryContext> repositoryContextFactory, ServiceLifetime serviceLifetime)
            : base(context, repositoryContextFactory, serviceLifetime)
        { }
    }
}
