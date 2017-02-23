using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Apworks.Repositories;

namespace Apworks.Integration.AspNetCore.Configuration
{
    /// <summary>
    /// Represents that the implemented classes are configurators that
    /// configures the repository in the service collection.
    /// </summary>
    /// <seealso cref="Apworks.Integration.AspNetCore.Configuration.IConfigurator" />
    public interface IRepositoryConfigurator : IConfigurator
    {
    }

    internal sealed class RepositoryConfigurator<TRepositoryContext> : ServiceRegisterConfigurator<IRepositoryContext, TRepositoryContext>, IRepositoryConfigurator
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
