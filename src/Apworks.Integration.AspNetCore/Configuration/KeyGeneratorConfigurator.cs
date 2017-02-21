using Apworks.KeyGeneration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Apworks.Integration.AspNetCore.Configuration
{
    public interface IKeyGeneratorConfigurator : IConfigurator
    {

    }

    internal sealed class KeyGeneratorConfigurator<TKey, TAggregateRoot, TKeyGenerator> 
        : ServiceRegisterConfigurator<IKeyGenerator<TKey, TAggregateRoot>, TKeyGenerator>, IKeyGeneratorConfigurator
        where TKey : IEquatable<TKey>
        where TAggregateRoot : class, IAggregateRoot<TKey>
        where TKeyGenerator : class, IKeyGenerator<TKey, TAggregateRoot>
    {
        public KeyGeneratorConfigurator(IConfigurator context, TKeyGenerator implementation, ServiceLifetime serviceLifetime)
            : base(context, implementation, serviceLifetime)
        { }

        public KeyGeneratorConfigurator(IConfigurator context, Func<IServiceProvider, TKeyGenerator> implementationFactory, ServiceLifetime serviceLifetime)
            : base(context, implementationFactory, serviceLifetime)
        { }
    }
}
