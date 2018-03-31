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
// Copyright (C) 2009-2018 by daxnet.
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
using Apworks.Integration.AspNetCore.DataServices;
using Apworks.Integration.AspNetCore.Hal;
using Apworks.KeyGeneration;
using Apworks.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Apworks.Integration.AspNetCore
{
    /// <summary>
    /// Provides the extension methods for integrating the Apworks facilities into ASP.NET Core MVC/Web API stack.
    /// </summary>
    public static class IntegrationExtensions
    {
        #region IServiceCollection Extensions

        /// <summary>
        /// Extends the ASP.NET Core application to add the Apworks capabilities.
        /// </summary>
        /// <param name="serviceCollection">The service collection to which the Apworks capability will be added.</param>
        /// <returns>The configurator instance which registers the required service to the <see cref="IServiceCollection"/> instance.</returns>
        public static IApworksConfigurator AddApworks(this IServiceCollection serviceCollection)
        {
            return new ApworksConfigurator(serviceCollection);
        }

        #endregion IServiceCollection Extensions

        #region IApworksConfigurator Extensions

        /// <summary>
        /// Adds the repository capability to the Apworks application.
        /// </summary>
        /// <param name="configurator">The <see cref="IApworksConfigurator"/> which configures the Apworks application.</param>
        /// <param name="repositoryContext">The <see cref="IRepositoryContext"/> which provides the repository executing context and manages transactions.</param>
        /// <param name="serviceLifetime">The service lifetime which represents the lifetime of the registered repository context.</param>
        /// <returns>The <see cref="IRepositoryConfigurator"/> which configures the Apworks application.</returns>
        public static IRepositoryConfigurator WithRepository(this IApworksConfigurator configurator, IRepositoryContext repositoryContext, ServiceLifetime serviceLifetime = ServiceLifetime.Scoped)
        {
            return new RepositoryConfigurator<IRepositoryContext>(configurator, repositoryContext, serviceLifetime);
        }

        /// <summary>
        /// Adds the repository capability to the Apworks application.
        /// </summary>
        /// <param name="configurator">The <see cref="IApworksConfigurator"/> which configures the Apworks application.</param>
        /// <param name="repositoryContextFactory">The factory delegate that creates the instance of <see cref="IRepositoryContext"/>.</param>
        /// <param name="serviceLifetime">The service lifetime which represents the lifetime of the registered repository context.</param>
        /// <returns>The <see cref="IRepositoryConfigurator"/> which configures the Apworks application.</returns>
        public static IRepositoryConfigurator WithRepository(this IApworksConfigurator configurator, Func<IServiceProvider, IRepositoryContext> repositoryContextFactory, ServiceLifetime serviceLifetime = ServiceLifetime.Scoped)
        {
            return new RepositoryConfigurator<IRepositoryContext>(configurator, repositoryContextFactory, serviceLifetime);
        }

        /// <summary>
        /// Adds the data service support to the Apworks application.
        /// </summary>
        /// <param name="configurator">The <see cref="IApworksConfigurator"/> which configures the Apworks application.</param>
        /// <param name="options">The <see cref="DataServiceConfigurationOptions"/> that specifies the options for creating the data service.</param>
        /// <returns>The <see cref="IDataServiceConfigurator"/> which configures the Apworks application.</returns>
        public static IDataServiceConfigurator WithDataServiceSupport(this IApworksConfigurator configurator, DataServiceConfigurationOptions options)
        {
            return new DataServiceConfigurator(configurator, options);
        }

        /// <summary>
        /// Adds the Hypertext Application Language (HAL) support to the Apworks application.
        /// </summary>
        /// <typeparam name="THalBuildConfiguration">The type of the HAL build configuration.</typeparam>
        /// <param name="configurator">The <see cref="IApworksConfigurator"/> which configures the Apworks application.</param>
        /// <param name="halBuildConfiguration">The <see cref="IHalBuildConfiguration"/> instance which defines the logic of building the HAL resource.</param>
        /// <param name="serviceLifetime">The service lifetime which represents the lifetime of the registered HAL build configuration.</param>
        /// <returns>The <see cref="IHalSupportConfigurator"/> which configures the Apworks application.</returns>
        public static IHalSupportConfigurator WithHalSupport<THalBuildConfiguration>(this IApworksConfigurator configurator, THalBuildConfiguration halBuildConfiguration, ServiceLifetime serviceLifetime = ServiceLifetime.Scoped)
            where THalBuildConfiguration : class, IHalBuildConfiguration
        {
            return new HalSupportConfigurator<THalBuildConfiguration>(configurator, halBuildConfiguration, serviceLifetime);
        }

        public static IHalSupportConfigurator WithHalSupport<THalBuildConfiguration>(this IApworksConfigurator configurator, Func<IServiceProvider, THalBuildConfiguration> halBuildConfigurationFactory, ServiceLifetime serviceLifetime = ServiceLifetime.Scoped)
            where THalBuildConfiguration : class, IHalBuildConfiguration
        {
            return new HalSupportConfigurator<THalBuildConfiguration>(configurator, halBuildConfigurationFactory, serviceLifetime);
        }

        #endregion IApworksConfigurator Extensions

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

        public static IHalSupportConfigurator WithHalSupport<THalBuildConfiguration>(this IRepositoryConfigurator configurator, THalBuildConfiguration halBuildConfiguration, ServiceLifetime serviceLifetime = ServiceLifetime.Scoped)
            where THalBuildConfiguration : class, IHalBuildConfiguration
        {
            return new HalSupportConfigurator<THalBuildConfiguration>(configurator, halBuildConfiguration, serviceLifetime);
        }

        public static IHalSupportConfigurator WithHalSupport<THalBuildConfiguration>(this IRepositoryConfigurator configurator, Func<IServiceProvider, THalBuildConfiguration> halBuildConfigurationFactory, ServiceLifetime serviceLifetime = ServiceLifetime.Scoped)
            where THalBuildConfiguration : class, IHalBuildConfiguration
        {
            return new HalSupportConfigurator<THalBuildConfiguration>(configurator, halBuildConfigurationFactory, serviceLifetime);
        }

        #endregion IRepositoryConfigurator Extensions

        #region IKeyGeneratorConfigurator Extensions

        public static IKeyGeneratorConfigurator WithKeyGenerator<TKey, TAggregateRoot>(this IKeyGeneratorConfigurator configurator, IKeyGenerator<TKey, TAggregateRoot> keyGenerator, ServiceLifetime serviceLifetime = ServiceLifetime.Scoped)
            where TKey : IEquatable<TKey>
            where TAggregateRoot : class, IAggregateRoot<TKey>
        {
            return new KeyGeneratorConfigurator<TKey, TAggregateRoot, IKeyGenerator<TKey, TAggregateRoot>>(configurator, keyGenerator, serviceLifetime);
        }

        public static IKeyGeneratorConfigurator WithKeyGenerator<TKey, TAggregateRoot>(this IKeyGeneratorConfigurator configurator, Func<IServiceProvider, IKeyGenerator<TKey, TAggregateRoot>> keyGeneratorFactory, ServiceLifetime serviceLifetime = ServiceLifetime.Scoped)
            where TKey : IEquatable<TKey>
            where TAggregateRoot : class, IAggregateRoot<TKey>
        {
            return new KeyGeneratorConfigurator<TKey, TAggregateRoot, IKeyGenerator<TKey, TAggregateRoot>>(configurator, keyGeneratorFactory, serviceLifetime);
        }

        public static IHalSupportConfigurator WithHalSupport<THalBuildConfiguration>(this IKeyGeneratorConfigurator configurator, THalBuildConfiguration halBuildConfiguration, ServiceLifetime serviceLifetime = ServiceLifetime.Scoped)
            where THalBuildConfiguration : class, IHalBuildConfiguration
        {
            return new HalSupportConfigurator<THalBuildConfiguration>(configurator, halBuildConfiguration, serviceLifetime);
        }

        public static IHalSupportConfigurator WithHalSupport<THalBuildConfiguration>(this IKeyGeneratorConfigurator configurator, Func<IServiceProvider, THalBuildConfiguration> halBuildConfigurationFactory, ServiceLifetime serviceLifetime = ServiceLifetime.Scoped)
            where THalBuildConfiguration : class, IHalBuildConfiguration
        {
            return new HalSupportConfigurator<THalBuildConfiguration>(configurator, halBuildConfigurationFactory, serviceLifetime);
        }

        #endregion IKeyGeneratorConfigurator Extensions

        #region IApplicationBuilder Extensions

        public static IApplicationBuilder EnrichDataServiceExceptionResponse(this IApplicationBuilder applicationBuilder)
        {
            return applicationBuilder.UseMiddleware<DataServiceExceptionHandler>();
        }

        #endregion IApplicationBuilder Extensions
    }
}