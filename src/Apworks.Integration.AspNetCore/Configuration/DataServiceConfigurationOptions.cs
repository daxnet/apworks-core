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

using Apworks.Integration.AspNetCore.DataServices;
using Apworks.Integration.AspNetCore.Hal;
using Apworks.KeyGeneration;
using Apworks.Querying;
using Apworks.Querying.Parsers.Irony;
using Apworks.Repositories;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace Apworks.Integration.AspNetCore.Configuration
{
    /// <summary>
    /// Represents the data that contains the options for the data service configuration.
    /// </summary>
    public sealed class DataServiceConfigurationOptions
    {
        #region Private Fields
        private readonly List<Tuple<Type, Func<IServiceProvider, object>, ServiceLifetime>> serviceFactoryRegistrations = new List<Tuple<Type, Func<IServiceProvider, object>, ServiceLifetime>>();
        #endregion

        #region Ctor
        /// <summary>
        /// Initializes a new instance of the <see cref="DataServiceConfigurationOptions"/> class.
        /// </summary>
        /// <param name="repositoryContext">The repository context.</param>
        /// <param name="halBuildConfigurationFactory">The hal build configuration factory.</param>
        public DataServiceConfigurationOptions(IRepositoryContext repositoryContext, Func<IServiceProvider, IHalBuildConfiguration> halBuildConfigurationFactory)
            : this(repositoryContext, ServiceLifetime.Scoped, true, halBuildConfigurationFactory)
        { }

        public DataServiceConfigurationOptions(Func<IServiceProvider, IRepositoryContext> repositoryContextFactory,
            Func<IServiceProvider, IHalBuildConfiguration> halBuildConfigurationFactory)
            : this(repositoryContextFactory, ServiceLifetime.Scoped, true, halBuildConfigurationFactory)
        { }

        public DataServiceConfigurationOptions(IRepositoryContext repositoryContext,
            Func<IServiceProvider, IQueryConditionParser> queryConditionParserFactory,
            Func<IServiceProvider, ISortSpecificationParser> sortSpecificationParserFactory)
            : this(repositoryContext, ServiceLifetime.Scoped, true, null, queryConditionParserFactory, sortSpecificationParserFactory)
        { }

        public DataServiceConfigurationOptions(Func<IServiceProvider, IRepositoryContext> repositoryContextFactory,
            Func<IServiceProvider, IQueryConditionParser> queryConditionParserFactory,
            Func<IServiceProvider, ISortSpecificationParser> sortSpecificationParserFactory)
            : this(repositoryContextFactory, ServiceLifetime.Scoped, true, null, queryConditionParserFactory, sortSpecificationParserFactory)
        { }

        public DataServiceConfigurationOptions(IRepositoryContext repositoryContext,
            ServiceLifetime repositoryContextLifetime = ServiceLifetime.Scoped,
            bool useHalSupport = true,
            Func<IServiceProvider, IHalBuildConfiguration> halBuildConfigurationFactory = null,
            Func<IServiceProvider, IQueryConditionParser> queryConditionParserFactory = null,
            Func<IServiceProvider, ISortSpecificationParser> sortSpecificationParserFactory = null)
            : this(_ => repositoryContext, repositoryContextLifetime, useHalSupport, halBuildConfigurationFactory,
                  queryConditionParserFactory, sortSpecificationParserFactory)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataServiceConfigurationOptions"/> class.
        /// </summary>
        /// <param name="repositoryContextFactory">The factory delegate that creates a <see cref="IRepositoryContext"/> instance.</param>
        /// <param name="repositoryContextLifetime">The lifetime of the <see cref="IRepositoryContext"/> instance that is used by the data service.</param>
        /// <param name="useHalSupport"><c>True</c> if Hypertext Application Language (HAL) should be applied to the response of the data service via HTTP GET requests,
        /// otherwise, <c>False</c>.</param>
        /// <param name="halBuildConfigurationFactory">The factory delegate that creates the configuration data which will be used when generating the HAL data structure.</param>
        public DataServiceConfigurationOptions(Func<IServiceProvider, IRepositoryContext> repositoryContextFactory,
            ServiceLifetime repositoryContextLifetime = ServiceLifetime.Scoped,
            bool useHalSupport = true,
            Func<IServiceProvider, IHalBuildConfiguration> halBuildConfigurationFactory = null,
            Func<IServiceProvider, IQueryConditionParser> queryConditionParserFactory = null,
            Func<IServiceProvider, ISortSpecificationParser> sortSpecificationParserFactory = null)
        {
            this.RepositoryContextFactory = repositoryContextFactory;
            this.RepositoryContextLifetime = repositoryContextLifetime;
            this.UseHalSupport = useHalSupport;
            this.HalBuildConfigurationFactory = halBuildConfigurationFactory == null ? _ => new DataServiceHalBuildConfiguration() : halBuildConfigurationFactory;
            this.QueryConditionParserFactory = queryConditionParserFactory == null ? _ => new IronyQueryConditionParser() : queryConditionParserFactory;
            this.SortSpecificationParserFactory = sortSpecificationParserFactory == null ? _ => new IronySortSpecificationParser() : sortSpecificationParserFactory;
        }
        #endregion

        #region Internal Properties
        /// <summary>
        /// Gets the lifetime of the <see cref="IRepositoryContext"/> instance that is used by the data service.
        /// </summary>
        /// <value>
        /// The lifetime of the <see cref="IRepositoryContext"/> instance that is used by the data service.
        /// </value>
        internal ServiceLifetime RepositoryContextLifetime { get; }

        /// <summary>
        /// Gets the factory delegate that creates a <see cref="IRepositoryContext"/> instance.
        /// </summary>
        /// <value>
        /// The factory delegate that creates a <see cref="IRepositoryContext"/> instance.
        /// </value>
        internal Func<IServiceProvider, IRepositoryContext> RepositoryContextFactory { get; }

        /// <summary>
        /// Gets a value indicating whether the Hypertext Application Language (HAL) should be applied to the response of the data service via HTTP GET requests.
        /// </summary>
        /// <value>
        ///   <c>True</c> if Hypertext Application Language (HAL) should be applied to the response of the data service via HTTP GET requests,
        /// otherwise, <c>False</c>.
        /// </value>
        internal bool UseHalSupport { get; }

        /// <summary>
        /// Gets the factory delegate that creates the configuration data which will be used when generating the HAL data structure.
        /// </summary>
        /// <value>
        /// The factory delegate that creates the configuration data which will be used when generating the HAL data structure.
        /// </value>
        internal Func<IServiceProvider, IHalBuildConfiguration> HalBuildConfigurationFactory { get; }

        internal Func<IServiceProvider, IQueryConditionParser> QueryConditionParserFactory { get; }

        internal Func<IServiceProvider, ISortSpecificationParser> SortSpecificationParserFactory { get; }

        /// <summary>
        /// Gets the service factory registrations that contains a list of the service factories that are going to be registered
        /// against the <see cref="IServiceCollection"/> instance.
        /// </summary>
        /// <value>
        /// The service factory registrations.
        /// </value>
        internal IEnumerable<Tuple<Type, Func<IServiceProvider, object>, ServiceLifetime>> ServiceFactoryRegistrations => serviceFactoryRegistrations;

        #endregion

        #region Public Methods
        /// <summary>
        /// Adds a service by using its factory to the registration.
        /// </summary>
        /// <typeparam name="TService">The type of the service whose factory delegate method will be added to the registration.</typeparam>
        /// <param name="factory">The factory delegate which creates the service instance.</param>
        /// <param name="serviceLifetime">The object lifetime of the service to be registered.</param>
        /// <returns>The current instance of <see cref="DataServiceConfigurationOptions"/> class.</returns>
        public DataServiceConfigurationOptions RegisterServiceFactory<TService>(Func<IServiceProvider, TService> factory, ServiceLifetime serviceLifetime = ServiceLifetime.Scoped)
            where TService : class
        {
            this.serviceFactoryRegistrations.Add(new Tuple<Type, Func<IServiceProvider, object>, ServiceLifetime>(typeof(TService), factory, serviceLifetime));
            return this;
        }

        /// <summary>
        /// Registers a service by using the service instance to the registration.
        /// </summary>
        /// <typeparam name="TService">The type of the service whose factory delegate method will be added to the registration.</typeparam>
        /// <param name="service">The service instance to be registered.</param>
        /// <param name="serviceLifetime">The object lifetime of the service to be registered.</param>
        /// <returns>The current instance of <see cref="DataServiceConfigurationOptions"/> class.</returns>
        public DataServiceConfigurationOptions RegisterService<TService>(TService service, ServiceLifetime serviceLifetime = ServiceLifetime.Scoped)
            where TService : class => this.RegisterServiceFactory<TService>(_ => service, serviceLifetime);

        /// <summary>
        /// Registers an <see cref="IKeyGenerator{TKey, TAggregateRoot}"/> instance by using the factory delegate to the service registration.
        /// </summary>
        /// <typeparam name="TKey">The type of the aggregate root key.</typeparam>
        /// <typeparam name="TAggregateRoot">The type of the aggregate root.</typeparam>
        /// <param name="keyGeneratorFactory">The key generator factory which creates an instance of <see cref="IKeyGenerator{TKey, TAggregateRoot}"/> class.</param>
        /// <returns>The current instance of <see cref="DataServiceConfigurationOptions"/> class.</returns>
        public DataServiceConfigurationOptions RegisterKeyGenerator<TKey, TAggregateRoot>(Func<IServiceProvider, IKeyGenerator<TKey, TAggregateRoot>> keyGeneratorFactory)
            where TKey : IEquatable<TKey>
            where TAggregateRoot : class, IAggregateRoot<TKey> => this.RegisterServiceFactory<IKeyGenerator<TKey, TAggregateRoot>>(keyGeneratorFactory, ServiceLifetime.Singleton);

        /// <summary>
        /// Registers an <see cref="IKeyGenerator{TKey, TAggregateRoot}"/> instance by using its instance to the service registration.
        /// </summary>
        /// <typeparam name="TKey">The type of the aggregate root key.</typeparam>
        /// <typeparam name="TAggregateRoot">The type of the aggregate root.</typeparam>
        /// <param name="keyGenerator">The key generator instance to be registered.</param>
        /// <returns>The current instance of <see cref="DataServiceConfigurationOptions"/> class.</returns>
        public DataServiceConfigurationOptions RegisterKeyGenerator<TKey, TAggregateRoot>(IKeyGenerator<TKey, TAggregateRoot> keyGenerator)
            where TKey : IEquatable<TKey>
            where TAggregateRoot : class, IAggregateRoot<TKey> => this.RegisterService(keyGenerator, ServiceLifetime.Singleton);

        #endregion
    }
}
