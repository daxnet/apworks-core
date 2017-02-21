// ==================================================================================================================                                                                                          
//        ,::i                                                           BBB                
//       BBBBBi                                                         EBBB                
//      MBBNBBU                                                         BBB,                
//     BBB. BBB     BBB,BBBBM   BBB   UBBB   MBB,  LBBBBBO,   :BBG,BBB :BBB  .BBBU  kBBBBBF 
//    BBB,  BBB    7BBBBS2BBBO  BBB  iBBBB  YBBJ :BBBMYNBBB:  FBBBBBB: OBB: 5BBB,  BBBi ,M, 
//   MBBY   BBB.   8BBB   :BBB  BBB .BBUBB  BB1  BBBi   kBBB  BBBM     BBBjBBBr    BBB1     
//  BBBBBBBBBBBu   BBB    FBBP  MBM BB. BB BBM  7BBB    MBBY .BBB     7BBGkBB1      JBBBBi  
// PBBBFE0GkBBBB  7BBX   uBBB   MBBMBu .BBOBB   rBBB   kBBB  ZBBq     BBB: BBBJ   .   iBBB  
//BBBB      iBBB  BBBBBBBBBE    EBBBB  ,BBBB     MBBBBBBBM   BBB,    iBBB  .BBB2 :BBBBBBB7  
//vr7        777  BBBu8O5:      .77r    Lr7       .7EZk;     L77     .Y7r   irLY  JNMMF:    
//               LBBj
//
// Apworks Application Development Framework
// Copyright (C) 2009-2017 by daxnet.
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//    http://www.apache.org/licenses/LICENSE-2.0
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// ==================================================================================================================

using Apworks.Integration.AspNetCore.Configuration;
using Apworks.KeyGeneration;
using Apworks.Repositories;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Apworks.Integration.AspNetCore
{
    /// <summary>
    /// Provides the extension methods for integrating the Apworks facilities into ASP.NET Core MVC/Web API stack.
    /// </summary>
    public static class IntegrationExtensions
    {
        #region Commented
        /// <summary>
        /// Adds a <see cref="IRepositoryContext"/> instance to the service collection.
        /// </summary>
        /// <typeparam name="TRepositoryContext">The type of the repository context.</typeparam>
        /// <param name="serviceCollection">The service collection to which the instance is going to be added.</param>
        /// <param name="instance">The instance of <see cref="IRepositoryContext"/> that needs to be added to the IoC container.</param>
        /// <param name="lifetime">The lifetime of the instance.</param>
        /// <returns></returns>
        //public static IServiceCollection AddRepository<TRepositoryContext>(this IServiceCollection serviceCollection, TRepositoryContext instance, ServiceLifetime lifetime = ServiceLifetime.Scoped)
        //    where TRepositoryContext : class, IRepositoryContext
        //{
        //    switch(lifetime)
        //    {
        //        case ServiceLifetime.Scoped:
        //            return serviceCollection.AddScoped<IRepositoryContext>(serviceProvider => instance);
        //        case ServiceLifetime.Singleton:
        //            return serviceCollection.AddSingleton<IRepositoryContext>(serviceProvider => instance);
        //        default:
        //            return serviceCollection.AddTransient<IRepositoryContext>(serviceProvider => instance);
        //    }
        //}

        /// <summary>
        /// Adds a <see cref="IRepositoryContext"/> instance to the service collection.
        /// </summary>
        /// <typeparam name="TRepositoryContext">The type of the repository context.</typeparam>
        /// <param name="serviceCollection">The service collection to which the instance is going to be added.</param>
        /// <param name="instanceFactory">The factory that creates the <see cref="IRepositoryContext"/> instance to be added by using a given <see cref="IServiceProvider"/>.</param>
        /// <param name="lifetime">The lifetime of the instance.</param>
        /// <returns></returns>
        //public static IServiceCollection AddRepository<TRepositoryContext>(this IServiceCollection serviceCollection, Func<IServiceProvider, IRepositoryContext> instanceFactory, ServiceLifetime lifetime = ServiceLifetime.Scoped)
        //    where TRepositoryContext : class, IRepositoryContext
        //{
        //    switch(lifetime)
        //    {
        //        case ServiceLifetime.Scoped:
        //            return serviceCollection.AddScoped<IRepositoryContext>(instanceFactory);
        //        case ServiceLifetime.Singleton:
        //            return serviceCollection.AddSingleton<IRepositoryContext>(instanceFactory);
        //        default:
        //            return serviceCollection.AddTransient<IRepositoryContext>(instanceFactory);
        //    }
        //}
        #endregion

        #region IServiceCollection Extensions
        public static IApworksConfigurator AddApworks(this IServiceCollection serviceCollection)
        {
            return new ApworksConfigurator(serviceCollection);
        }
        #endregion

        #region IApworksConfigurator Extensions
        public static IRepositoryConfigurator WithRepository(this IApworksConfigurator configurator, IRepositoryContext repositoryContext, ServiceLifetime serviceLifetime = ServiceLifetime.Scoped)
        {
            return new RepositoryConfigurator<IRepositoryContext>(configurator, repositoryContext, serviceLifetime);
        }

        public static IRepositoryConfigurator WithRepository(this IApworksConfigurator configurator, Func<IServiceProvider, IRepositoryContext> repositoryContextFactory, ServiceLifetime serviceLifetime = ServiceLifetime.Scoped)
        {
            return new RepositoryConfigurator<IRepositoryContext>(configurator, repositoryContextFactory, serviceLifetime);
        }

        public static IKeyGeneratorConfigurator WithKeyGenerator<TKey, TAggregateRoot>(this IApworksConfigurator configurator, IKeyGenerator<TKey, TAggregateRoot> keyGenerator, ServiceLifetime serviceLifetime = ServiceLifetime.Scoped)
            where TKey : IEquatable<TKey>
            where TAggregateRoot : class, IAggregateRoot<TKey>
        {
            return new KeyGeneratorConfigurator<TKey, TAggregateRoot, IKeyGenerator<TKey, TAggregateRoot>>(configurator, keyGenerator, serviceLifetime);
        }

        public static IKeyGeneratorConfigurator WithKeyGenerator<TKey, TAggregateRoot>(this IApworksConfigurator configurator, Func<IServiceProvider, IKeyGenerator<TKey, TAggregateRoot>> keyGeneratorFactory, ServiceLifetime serviceLifetime = ServiceLifetime.Scoped)
            where TKey : IEquatable<TKey>
            where TAggregateRoot : class, IAggregateRoot<TKey>
        {
            return new KeyGeneratorConfigurator<TKey, TAggregateRoot, IKeyGenerator<TKey, TAggregateRoot>>(configurator, keyGeneratorFactory, serviceLifetime);
        }
        #endregion

        #region IRepositoryConfigurator Extensions
        public static IKeyGeneratorConfigurator WithKeyGenerator<TKey, TAggregateRoot>(this IRepositoryConfigurator configurator, IKeyGenerator<TKey, TAggregateRoot> keyGenerator, ServiceLifetime serviceLifetime = ServiceLifetime.Scoped)
            where TKey : IEquatable<TKey>
            where TAggregateRoot : class, IAggregateRoot<TKey>
        {
            return new KeyGeneratorConfigurator<TKey, TAggregateRoot, IKeyGenerator<TKey, TAggregateRoot>>(configurator, keyGenerator, serviceLifetime);
        }

        public static IKeyGeneratorConfigurator WithKeyGenerator<TKey, TAggregateRoot>(this IRepositoryConfigurator configurator, Func<IServiceProvider, IKeyGenerator<TKey, TAggregateRoot>> keyGeneratorFactory, ServiceLifetime serviceLifetime = ServiceLifetime.Scoped)
            where TKey : IEquatable<TKey>
            where TAggregateRoot : class, IAggregateRoot<TKey>
        {
            return new KeyGeneratorConfigurator<TKey, TAggregateRoot, IKeyGenerator<TKey, TAggregateRoot>>(configurator, keyGeneratorFactory, serviceLifetime);
        }
        #endregion
    }
}
