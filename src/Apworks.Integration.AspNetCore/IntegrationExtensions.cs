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

using Apworks.Commands;
using Apworks.Events;
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
        #endregion

        #region IApworksConfigurator Extensions        
        /// <summary>
        /// Extends the Apworks capabilities with the repository support.
        /// </summary>
        /// <param name="configurator">The configurator instance which registers the <see cref="IRepositoryContext"/> instance to the <see cref="IServiceCollection"/> instance.</param>
        /// <param name="repositoryContext">The repository context.</param>
        /// <param name="serviceLifetime">The service lifetime.</param>
        /// <returns></returns>
        public static IRepositoryConfigurator WithRepository(this IApworksConfigurator configurator, IRepositoryContext repositoryContext, ServiceLifetime serviceLifetime = ServiceLifetime.Scoped)
        {
            return new RepositoryConfigurator<IRepositoryContext>(configurator, repositoryContext, serviceLifetime);
        }

        public static IRepositoryConfigurator WithRepository(this IApworksConfigurator configurator, Func<IServiceProvider, IRepositoryContext> repositoryContextFactory, ServiceLifetime serviceLifetime = ServiceLifetime.Scoped)
        {
            return new RepositoryConfigurator<IRepositoryContext>(configurator, repositoryContextFactory, serviceLifetime);
        }

        public static IDomainRepositoryConfigurator WithDomainRepository<TDomainRepository>(this IApworksConfigurator configurator, TDomainRepository domainRepository, ServiceLifetime serviceLifetime = ServiceLifetime.Scoped)
            where TDomainRepository : class, IDomainRepository
        {
            return new DomainRepositoryConfigurator<TDomainRepository>(configurator, domainRepository, serviceLifetime);
        }

        public static IDomainRepositoryConfigurator WithDomainRepository<TDomainRepository>(this IApworksConfigurator configurator, Func<IServiceProvider, TDomainRepository> domainRepositoryFactory, ServiceLifetime serviceLifetime = ServiceLifetime.Scoped)
            where TDomainRepository : class, IDomainRepository
        {
            return new DomainRepositoryConfigurator<TDomainRepository>(configurator, domainRepositoryFactory, serviceLifetime);
        }

        public static IDataServiceConfigurator WithDataServiceSupport(this IApworksConfigurator configurator, DataServiceConfigurationOptions options)
        {
            return new DataServiceConfigurator(configurator, options);
        }

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

        public static IEventBusConfigurator WithEventBus<TEventBus>(this IApworksConfigurator configurator, TEventBus eventBus, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
            where TEventBus : class, IEventBus
        {
            return new EventBusConfigurator<TEventBus>(configurator, eventBus, serviceLifetime);
        }

        public static IEventBusConfigurator WithEventBus<TEventBus>(this IApworksConfigurator configurator, Func<IServiceProvider, TEventBus> eventBusFactory, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
            where TEventBus : class, IEventBus
        {
            return new EventBusConfigurator<TEventBus>(configurator, eventBusFactory, serviceLifetime);
        }

        public static IEventSubscriberConfigurator WithEventSubscriber<TEventSubscriber>(this IApworksConfigurator configurator, TEventSubscriber eventSubscriber, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
            where TEventSubscriber : class, IEventSubscriber
        {
            return new EventSubscriberConfigurator<TEventSubscriber>(configurator, eventSubscriber, serviceLifetime);
        }

        public static IEventSubscriberConfigurator WithEventSubscriber<TEventSubscriber>(this IApworksConfigurator configurator, Func<IServiceProvider, TEventSubscriber> eventSubscriberFactory, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
            where TEventSubscriber : class, IEventSubscriber
        {
            return new EventSubscriberConfigurator<TEventSubscriber>(configurator, eventSubscriberFactory, serviceLifetime);
        }

        public static IEventPublisherConfigurator WithEventPublisher<TEventPublisher>(this IApworksConfigurator configurator, TEventPublisher eventPublisher, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
            where TEventPublisher : class, IEventPublisher
        {
            return new EventPublisherConfigurator<TEventPublisher>(configurator, eventPublisher, serviceLifetime);
        }

        public static IEventPublisherConfigurator WithEventPublisher<TEventPublisher>(this IApworksConfigurator configurator, Func<IServiceProvider, TEventPublisher> eventPublisherFactory, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
            where TEventPublisher : class, IEventPublisher
        {
            return new EventPublisherConfigurator<TEventPublisher>(configurator, eventPublisherFactory, serviceLifetime);
        }

        public static ICommandBusConfigurator WithCommandBus<TCommandBus>(this IApworksConfigurator configurator, TCommandBus commandBus, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
            where TCommandBus : class, ICommandBus
        {
            return new CommandBusConfigurator<TCommandBus>(configurator, commandBus, serviceLifetime);
        }

        public static ICommandBusConfigurator WithCommandBus<TCommandBus>(this IApworksConfigurator configurator, Func<IServiceProvider, TCommandBus> commandBusFactory, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
            where TCommandBus : class, ICommandBus
        {
            return new CommandBusConfigurator<TCommandBus>(configurator, commandBusFactory, serviceLifetime);
        }

        public static ICommandSenderConfigurator WithCommandSender<TCommandSender>(this IApworksConfigurator configurator, TCommandSender commandSender, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
            where TCommandSender : class, ICommandSender
        {
            return new CommandSenderConfigurator<TCommandSender>(configurator, commandSender, serviceLifetime);
        }

        public static ICommandSenderConfigurator WithCommandSender<TCommandSender>(this IApworksConfigurator configurator, Func<IServiceProvider, TCommandSender> commandSenderFactory, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
            where TCommandSender : class, ICommandSender
        {
            return new CommandSenderConfigurator<TCommandSender>(configurator, commandSenderFactory, serviceLifetime);
        }

        public static ICommandSubscriberConfigurator WithCommandSubscriber<TCommandSubscriber>(this IApworksConfigurator configurator, TCommandSubscriber commandSubscriber, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
            where TCommandSubscriber : class, ICommandSubscriber
        {
            return new CommandSubscriberConfigurator<TCommandSubscriber>(configurator, commandSubscriber, serviceLifetime);
        }

        public static ICommandSubscriberConfigurator WithCommandSubscriber<TCommandSubscriber>(this IApworksConfigurator configurator, Func<IServiceProvider, TCommandSubscriber> commandSubscriberFactory, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
            where TCommandSubscriber : class, ICommandSubscriber
        {
            return new CommandSubscriberConfigurator<TCommandSubscriber>(configurator, commandSubscriberFactory, serviceLifetime);
        }
        #endregion

        #region IDomainRepositoryConfigurator Extensions

        public static IEventStoreConfigurator WithEventStore<TEventStore>(this IDomainRepositoryConfigurator configurator, TEventStore eventStore, ServiceLifetime serviceLifetime = ServiceLifetime.Scoped)
            where TEventStore : class, IEventStore
        {
            return new EventStoreConfigurator<TEventStore>(configurator, eventStore, serviceLifetime);
        }

        public static IEventStoreConfigurator WithEventStore<TEventStore>(this IDomainRepositoryConfigurator configurator, Func<IServiceProvider, TEventStore> eventStoreFactory, ServiceLifetime serviceLifetime = ServiceLifetime.Scoped)
            where TEventStore : class, IEventStore
        {
            return new EventStoreConfigurator<TEventStore>(configurator, eventStoreFactory, serviceLifetime);
        }

        /// <summary>
        /// Extends the Apworks capabilities with the repository support.
        /// </summary>
        /// <param name="configurator">The configurator instance which registers the <see cref="IRepositoryContext"/> instance to the <see cref="IServiceCollection"/> instance.</param>
        /// <param name="repositoryContext">The repository context.</param>
        /// <param name="serviceLifetime">The service lifetime.</param>
        /// <returns></returns>
        public static IRepositoryConfigurator WithRepository(this IDomainRepositoryConfigurator configurator, IRepositoryContext repositoryContext, ServiceLifetime serviceLifetime = ServiceLifetime.Scoped)
        {
            return new RepositoryConfigurator<IRepositoryContext>(configurator, repositoryContext, serviceLifetime);
        }

        public static IRepositoryConfigurator WithRepository(this IDomainRepositoryConfigurator configurator, Func<IServiceProvider, IRepositoryContext> repositoryContextFactory, ServiceLifetime serviceLifetime = ServiceLifetime.Scoped)
        {
            return new RepositoryConfigurator<IRepositoryContext>(configurator, repositoryContextFactory, serviceLifetime);
        }

        public static IDataServiceConfigurator WithDataServiceSupport(this IDomainRepositoryConfigurator configurator, DataServiceConfigurationOptions options)
        {
            return new DataServiceConfigurator(configurator, options);
        }

        public static IHalSupportConfigurator WithHalSupport<THalBuildConfiguration>(this IDomainRepositoryConfigurator configurator, THalBuildConfiguration halBuildConfiguration, ServiceLifetime serviceLifetime = ServiceLifetime.Scoped)
            where THalBuildConfiguration : class, IHalBuildConfiguration
        {
            return new HalSupportConfigurator<THalBuildConfiguration>(configurator, halBuildConfiguration, serviceLifetime);
        }

        public static IHalSupportConfigurator WithHalSupport<THalBuildConfiguration>(this IDomainRepositoryConfigurator configurator, Func<IServiceProvider, THalBuildConfiguration> halBuildConfigurationFactory, ServiceLifetime serviceLifetime = ServiceLifetime.Scoped)
            where THalBuildConfiguration : class, IHalBuildConfiguration
        {
            return new HalSupportConfigurator<THalBuildConfiguration>(configurator, halBuildConfigurationFactory, serviceLifetime);
        }

        public static IEventBusConfigurator WithEventBus<TEventBus>(this IDomainRepositoryConfigurator configurator, TEventBus eventBus, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
            where TEventBus : class, IEventBus
        {
            return new EventBusConfigurator<TEventBus>(configurator, eventBus, serviceLifetime);
        }

        public static IEventBusConfigurator WithEventBus<TEventBus>(this IDomainRepositoryConfigurator configurator, Func<IServiceProvider, TEventBus> eventBusFactory, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
            where TEventBus : class, IEventBus
        {
            return new EventBusConfigurator<TEventBus>(configurator, eventBusFactory, serviceLifetime);
        }

        public static IEventSubscriberConfigurator WithEventSubscriber<TEventSubscriber>(this IDomainRepositoryConfigurator configurator, TEventSubscriber eventSubscriber, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
            where TEventSubscriber : class, IEventSubscriber
        {
            return new EventSubscriberConfigurator<TEventSubscriber>(configurator, eventSubscriber, serviceLifetime);
        }

        public static IEventSubscriberConfigurator WithEventSubscriber<TEventSubscriber>(this IDomainRepositoryConfigurator configurator, Func<IServiceProvider, TEventSubscriber> eventSubscriberFactory, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
            where TEventSubscriber : class, IEventSubscriber
        {
            return new EventSubscriberConfigurator<TEventSubscriber>(configurator, eventSubscriberFactory, serviceLifetime);
        }

        public static IEventPublisherConfigurator WithEventPublisher<TEventPublisher>(this IDomainRepositoryConfigurator configurator, TEventPublisher eventPublisher, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
            where TEventPublisher : class, IEventPublisher
        {
            return new EventPublisherConfigurator<TEventPublisher>(configurator, eventPublisher, serviceLifetime);
        }

        public static IEventPublisherConfigurator WithEventPublisher<TEventPublisher>(this IDomainRepositoryConfigurator configurator, Func<IServiceProvider, TEventPublisher> eventPublisherFactory, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
            where TEventPublisher : class, IEventPublisher
        {
            return new EventPublisherConfigurator<TEventPublisher>(configurator, eventPublisherFactory, serviceLifetime);
        }

        public static ICommandBusConfigurator WithCommandBus<TCommandBus>(this IDomainRepositoryConfigurator configurator, TCommandBus commandBus, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
            where TCommandBus : class, ICommandBus
        {
            return new CommandBusConfigurator<TCommandBus>(configurator, commandBus, serviceLifetime);
        }

        public static ICommandBusConfigurator WithCommandBus<TCommandBus>(this IDomainRepositoryConfigurator configurator, Func<IServiceProvider, TCommandBus> commandBusFactory, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
            where TCommandBus : class, ICommandBus
        {
            return new CommandBusConfigurator<TCommandBus>(configurator, commandBusFactory, serviceLifetime);
        }

        public static ICommandSenderConfigurator WithCommandSender<TCommandSender>(this IDomainRepositoryConfigurator configurator, TCommandSender commandSender, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
            where TCommandSender : class, ICommandSender
        {
            return new CommandSenderConfigurator<TCommandSender>(configurator, commandSender, serviceLifetime);
        }

        public static ICommandSenderConfigurator WithCommandSender<TCommandSender>(this IDomainRepositoryConfigurator configurator, Func<IServiceProvider, TCommandSender> commandSenderFactory, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
            where TCommandSender : class, ICommandSender
        {
            return new CommandSenderConfigurator<TCommandSender>(configurator, commandSenderFactory, serviceLifetime);
        }

        public static ICommandSubscriberConfigurator WithCommandSubscriber<TCommandSubscriber>(this IDomainRepositoryConfigurator configurator, TCommandSubscriber commandSubscriber, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
            where TCommandSubscriber : class, ICommandSubscriber
        {
            return new CommandSubscriberConfigurator<TCommandSubscriber>(configurator, commandSubscriber, serviceLifetime);
        }

        public static ICommandSubscriberConfigurator WithCommandSubscriber<TCommandSubscriber>(this IDomainRepositoryConfigurator configurator, Func<IServiceProvider, TCommandSubscriber> commandSubscriberFactory, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
            where TCommandSubscriber : class, ICommandSubscriber
        {
            return new CommandSubscriberConfigurator<TCommandSubscriber>(configurator, commandSubscriberFactory, serviceLifetime);
        }
        #endregion

        #region IEventStoreConfigurator Extensions
        /// <summary>
        /// Extends the Apworks capabilities with the repository support.
        /// </summary>
        /// <param name="configurator">The configurator instance which registers the <see cref="IRepositoryContext"/> instance to the <see cref="IServiceCollection"/> instance.</param>
        /// <param name="repositoryContext">The repository context.</param>
        /// <param name="serviceLifetime">The service lifetime.</param>
        /// <returns></returns>
        public static IRepositoryConfigurator WithRepository(this IEventStoreConfigurator configurator, IRepositoryContext repositoryContext, ServiceLifetime serviceLifetime = ServiceLifetime.Scoped)
        {
            return new RepositoryConfigurator<IRepositoryContext>(configurator, repositoryContext, serviceLifetime);
        }

        public static IRepositoryConfigurator WithRepository(this IEventStoreConfigurator configurator, Func<IServiceProvider, IRepositoryContext> repositoryContextFactory, ServiceLifetime serviceLifetime = ServiceLifetime.Scoped)
        {
            return new RepositoryConfigurator<IRepositoryContext>(configurator, repositoryContextFactory, serviceLifetime);
        }

        public static IDataServiceConfigurator WithDataServiceSupport(this IEventStoreConfigurator configurator, DataServiceConfigurationOptions options)
        {
            return new DataServiceConfigurator(configurator, options);
        }

        public static IHalSupportConfigurator WithHalSupport<THalBuildConfiguration>(this IEventStoreConfigurator configurator, THalBuildConfiguration halBuildConfiguration, ServiceLifetime serviceLifetime = ServiceLifetime.Scoped)
            where THalBuildConfiguration : class, IHalBuildConfiguration
        {
            return new HalSupportConfigurator<THalBuildConfiguration>(configurator, halBuildConfiguration, serviceLifetime);
        }

        public static IHalSupportConfigurator WithHalSupport<THalBuildConfiguration>(this IEventStoreConfigurator configurator, Func<IServiceProvider, THalBuildConfiguration> halBuildConfigurationFactory, ServiceLifetime serviceLifetime = ServiceLifetime.Scoped)
            where THalBuildConfiguration : class, IHalBuildConfiguration
        {
            return new HalSupportConfigurator<THalBuildConfiguration>(configurator, halBuildConfigurationFactory, serviceLifetime);
        }

        public static IEventBusConfigurator WithEventBus<TEventBus>(this IEventStoreConfigurator configurator, TEventBus eventBus, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
            where TEventBus : class, IEventBus
        {
            return new EventBusConfigurator<TEventBus>(configurator, eventBus, serviceLifetime);
        }

        public static IEventBusConfigurator WithEventBus<TEventBus>(this IEventStoreConfigurator configurator, Func<IServiceProvider, TEventBus> eventBusFactory, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
            where TEventBus : class, IEventBus
        {
            return new EventBusConfigurator<TEventBus>(configurator, eventBusFactory, serviceLifetime);
        }

        public static IEventSubscriberConfigurator WithEventSubscriber<TEventSubscriber>(this IEventStoreConfigurator configurator, TEventSubscriber eventSubscriber, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
            where TEventSubscriber : class, IEventSubscriber
        {
            return new EventSubscriberConfigurator<TEventSubscriber>(configurator, eventSubscriber, serviceLifetime);
        }

        public static IEventSubscriberConfigurator WithEventSubscriber<TEventSubscriber>(this IEventStoreConfigurator configurator, Func<IServiceProvider, TEventSubscriber> eventSubscriberFactory, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
            where TEventSubscriber : class, IEventSubscriber
        {
            return new EventSubscriberConfigurator<TEventSubscriber>(configurator, eventSubscriberFactory, serviceLifetime);
        }

        public static IEventPublisherConfigurator WithEventPublisher<TEventPublisher>(this IEventStoreConfigurator configurator, TEventPublisher eventPublisher, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
            where TEventPublisher : class, IEventPublisher
        {
            return new EventPublisherConfigurator<TEventPublisher>(configurator, eventPublisher, serviceLifetime);
        }

        public static IEventPublisherConfigurator WithEventPublisher<TEventPublisher>(this IEventStoreConfigurator configurator, Func<IServiceProvider, TEventPublisher> eventPublisherFactory, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
            where TEventPublisher : class, IEventPublisher
        {
            return new EventPublisherConfigurator<TEventPublisher>(configurator, eventPublisherFactory, serviceLifetime);
        }

        public static ICommandBusConfigurator WithCommandBus<TCommandBus>(this IEventStoreConfigurator configurator, TCommandBus commandBus, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
            where TCommandBus : class, ICommandBus
        {
            return new CommandBusConfigurator<TCommandBus>(configurator, commandBus, serviceLifetime);
        }

        public static ICommandBusConfigurator WithCommandBus<TCommandBus>(this IEventStoreConfigurator configurator, Func<IServiceProvider, TCommandBus> commandBusFactory, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
            where TCommandBus : class, ICommandBus
        {
            return new CommandBusConfigurator<TCommandBus>(configurator, commandBusFactory, serviceLifetime);
        }

        public static ICommandSenderConfigurator WithCommandSender<TCommandSender>(this IEventStoreConfigurator configurator, TCommandSender commandSender, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
            where TCommandSender : class, ICommandSender
        {
            return new CommandSenderConfigurator<TCommandSender>(configurator, commandSender, serviceLifetime);
        }

        public static ICommandSenderConfigurator WithCommandSender<TCommandSender>(this IEventStoreConfigurator configurator, Func<IServiceProvider, TCommandSender> commandSenderFactory, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
            where TCommandSender : class, ICommandSender
        {
            return new CommandSenderConfigurator<TCommandSender>(configurator, commandSenderFactory, serviceLifetime);
        }

        public static ICommandSubscriberConfigurator WithCommandSubscriber<TCommandSubscriber>(this IEventStoreConfigurator configurator, TCommandSubscriber commandSubscriber, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
            where TCommandSubscriber : class, ICommandSubscriber
        {
            return new CommandSubscriberConfigurator<TCommandSubscriber>(configurator, commandSubscriber, serviceLifetime);
        }

        public static ICommandSubscriberConfigurator WithCommandSubscriber<TCommandSubscriber>(this IEventStoreConfigurator configurator, Func<IServiceProvider, TCommandSubscriber> commandSubscriberFactory, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
            where TCommandSubscriber : class, ICommandSubscriber
        {
            return new CommandSubscriberConfigurator<TCommandSubscriber>(configurator, commandSubscriberFactory, serviceLifetime);
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

        public static IEventBusConfigurator WithEventBus<TEventBus>(this IRepositoryConfigurator configurator, TEventBus eventBus, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
            where TEventBus : class, IEventBus
        {
            return new EventBusConfigurator<TEventBus>(configurator, eventBus, serviceLifetime);
        }

        public static IEventBusConfigurator WithEventBus<TEventBus>(this IRepositoryConfigurator configurator, Func<IServiceProvider, TEventBus> eventBusFactory, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
            where TEventBus : class, IEventBus
        {
            return new EventBusConfigurator<TEventBus>(configurator, eventBusFactory, serviceLifetime);
        }

        public static IEventPublisherConfigurator WithEventPublisher<TEventPublisher>(this IRepositoryConfigurator configurator, TEventPublisher eventPublisher, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
            where TEventPublisher : class, IEventPublisher
        {
            return new EventPublisherConfigurator<TEventPublisher>(configurator, eventPublisher, serviceLifetime);
        }

        public static IEventPublisherConfigurator WithEventPublisher<TEventPublisher>(this IRepositoryConfigurator configurator, Func<IServiceProvider, TEventPublisher> eventPublisherFactory, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
            where TEventPublisher : class, IEventPublisher
        {
            return new EventPublisherConfigurator<TEventPublisher>(configurator, eventPublisherFactory, serviceLifetime);
        }

        public static IEventSubscriberConfigurator WithEventSubscriber<TEventSubscriber>(this IRepositoryConfigurator configurator, TEventSubscriber eventSubscriber, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
            where TEventSubscriber : class, IEventSubscriber
        {
            return new EventSubscriberConfigurator<TEventSubscriber>(configurator, eventSubscriber, serviceLifetime);
        }

        public static IEventSubscriberConfigurator WithEventSubscriber<TEventSubscriber>(this IRepositoryConfigurator configurator, Func<IServiceProvider, TEventSubscriber> eventSubscriberFactory, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
            where TEventSubscriber : class, IEventSubscriber
        {
            return new EventSubscriberConfigurator<TEventSubscriber>(configurator, eventSubscriberFactory, serviceLifetime);
        }

        public static ICommandBusConfigurator WithCommandBus<TCommandBus>(this IRepositoryConfigurator configurator, TCommandBus commandBus, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
            where TCommandBus : class, ICommandBus
        {
            return new CommandBusConfigurator<TCommandBus>(configurator, commandBus, serviceLifetime);
        }

        public static ICommandBusConfigurator WithCommandBus<TCommandBus>(this IRepositoryConfigurator configurator, Func<IServiceProvider, TCommandBus> commandBusFactory, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
            where TCommandBus : class, ICommandBus
        {
            return new CommandBusConfigurator<TCommandBus>(configurator, commandBusFactory, serviceLifetime);
        }

        public static ICommandSenderConfigurator WithCommandSender<TCommandSender>(this IRepositoryConfigurator configurator, TCommandSender commandSender, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
            where TCommandSender : class, ICommandSender
        {
            return new CommandSenderConfigurator<TCommandSender>(configurator, commandSender, serviceLifetime);
        }

        public static ICommandSenderConfigurator WithCommandSender<TCommandSender>(this IRepositoryConfigurator configurator, Func<IServiceProvider, TCommandSender> commandSenderFactory, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
            where TCommandSender : class, ICommandSender
        {
            return new CommandSenderConfigurator<TCommandSender>(configurator, commandSenderFactory, serviceLifetime);
        }

        public static ICommandSubscriberConfigurator WithCommandSubscriber<TCommandSubscriber>(this IRepositoryConfigurator configurator, TCommandSubscriber commandSubscriber, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
            where TCommandSubscriber : class, ICommandSubscriber
        {
            return new CommandSubscriberConfigurator<TCommandSubscriber>(configurator, commandSubscriber, serviceLifetime);
        }

        public static ICommandSubscriberConfigurator WithCommandSubscriber<TCommandSubscriber>(this IRepositoryConfigurator configurator, Func<IServiceProvider, TCommandSubscriber> commandSubscriberFactory, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
            where TCommandSubscriber : class, ICommandSubscriber
        {
            return new CommandSubscriberConfigurator<TCommandSubscriber>(configurator, commandSubscriberFactory, serviceLifetime);
        }
        #endregion

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

        public static IEventBusConfigurator WithEventBus<TEventBus>(this IKeyGeneratorConfigurator configurator, TEventBus eventBus, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
            where TEventBus : class, IEventBus
        {
            return new EventBusConfigurator<TEventBus>(configurator, eventBus, serviceLifetime);
        }

        public static IEventBusConfigurator WithEventBus<TEventBus>(this IKeyGeneratorConfigurator configurator, Func<IServiceProvider, TEventBus> eventBusFactory, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
            where TEventBus : class, IEventBus
        {
            return new EventBusConfigurator<TEventBus>(configurator, eventBusFactory, serviceLifetime);
        }

        public static IEventPublisherConfigurator WithEventPublisher<TEventPublisher>(this IKeyGeneratorConfigurator configurator, TEventPublisher eventPublisher, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
            where TEventPublisher : class, IEventPublisher
        {
            return new EventPublisherConfigurator<TEventPublisher>(configurator, eventPublisher, serviceLifetime);
        }

        public static IEventPublisherConfigurator WithEventPublisher<TEventPublisher>(this IKeyGeneratorConfigurator configurator, Func<IServiceProvider, TEventPublisher> eventPublisherFactory, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
            where TEventPublisher : class, IEventPublisher
        {
            return new EventPublisherConfigurator<TEventPublisher>(configurator, eventPublisherFactory, serviceLifetime);
        }

        public static IEventSubscriberConfigurator WithEventSubscriber<TEventSubscriber>(this IKeyGeneratorConfigurator configurator, TEventSubscriber eventSubscriber, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
            where TEventSubscriber : class, IEventSubscriber
        {
            return new EventSubscriberConfigurator<TEventSubscriber>(configurator, eventSubscriber, serviceLifetime);
        }

        public static IEventSubscriberConfigurator WithEventSubscriber<TEventSubscriber>(this IKeyGeneratorConfigurator configurator, Func<IServiceProvider, TEventSubscriber> eventSubscriberFactory, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
            where TEventSubscriber : class, IEventSubscriber
        {
            return new EventSubscriberConfigurator<TEventSubscriber>(configurator, eventSubscriberFactory, serviceLifetime);
        }

        public static ICommandBusConfigurator WithCommandBus<TCommandBus>(this IKeyGeneratorConfigurator configurator, TCommandBus commandBus, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
            where TCommandBus : class, ICommandBus
        {
            return new CommandBusConfigurator<TCommandBus>(configurator, commandBus, serviceLifetime);
        }

        public static ICommandBusConfigurator WithCommandBus<TCommandBus>(this IKeyGeneratorConfigurator configurator, Func<IServiceProvider, TCommandBus> commandBusFactory, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
            where TCommandBus : class, ICommandBus
        {
            return new CommandBusConfigurator<TCommandBus>(configurator, commandBusFactory, serviceLifetime);
        }

        public static ICommandSenderConfigurator WithCommandSender<TCommandSender>(this IKeyGeneratorConfigurator configurator, TCommandSender commandSender, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
            where TCommandSender : class, ICommandSender
        {
            return new CommandSenderConfigurator<TCommandSender>(configurator, commandSender, serviceLifetime);
        }

        public static ICommandSenderConfigurator WithCommandSender<TCommandSender>(this IKeyGeneratorConfigurator configurator, Func<IServiceProvider, TCommandSender> commandSenderFactory, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
            where TCommandSender : class, ICommandSender
        {
            return new CommandSenderConfigurator<TCommandSender>(configurator, commandSenderFactory, serviceLifetime);
        }

        public static ICommandSubscriberConfigurator WithCommandSubscriber<TCommandSubscriber>(this IKeyGeneratorConfigurator configurator, TCommandSubscriber commandSubscriber, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
            where TCommandSubscriber : class, ICommandSubscriber
        {
            return new CommandSubscriberConfigurator<TCommandSubscriber>(configurator, commandSubscriber, serviceLifetime);
        }

        public static ICommandSubscriberConfigurator WithCommandSubscriber<TCommandSubscriber>(this IKeyGeneratorConfigurator configurator, Func<IServiceProvider, TCommandSubscriber> commandSubscriberFactory, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
            where TCommandSubscriber : class, ICommandSubscriber
        {
            return new CommandSubscriberConfigurator<TCommandSubscriber>(configurator, commandSubscriberFactory, serviceLifetime);
        }

        #endregion

        #region IHalSupportConfigurator Extensions
        public static IEventBusConfigurator WithEventBus<TEventBus>(this IHalSupportConfigurator configurator, TEventBus eventBus, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
            where TEventBus : class, IEventBus
        {
            return new EventBusConfigurator<TEventBus>(configurator, eventBus, serviceLifetime);
        }

        public static IEventBusConfigurator WithEventBus<TEventBus>(this IHalSupportConfigurator configurator, Func<IServiceProvider, TEventBus> eventBusFactory, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
            where TEventBus : class, IEventBus
        {
            return new EventBusConfigurator<TEventBus>(configurator, eventBusFactory, serviceLifetime);
        }

        public static IEventPublisherConfigurator WithEventPublisher<TEventPublisher>(this IHalSupportConfigurator configurator, TEventPublisher eventPublisher, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
            where TEventPublisher : class, IEventPublisher
        {
            return new EventPublisherConfigurator<TEventPublisher>(configurator, eventPublisher, serviceLifetime);
        }

        public static IEventPublisherConfigurator WithEventPublisher<TEventPublisher>(this IHalSupportConfigurator configurator, Func<IServiceProvider, TEventPublisher> eventPublisherFactory, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
            where TEventPublisher : class, IEventPublisher
        {
            return new EventPublisherConfigurator<TEventPublisher>(configurator, eventPublisherFactory, serviceLifetime);
        }

        public static IEventSubscriberConfigurator WithEventSubscriber<TEventSubscriber>(this IHalSupportConfigurator configurator, TEventSubscriber eventSubscriber, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
            where TEventSubscriber : class, IEventSubscriber
        {
            return new EventSubscriberConfigurator<TEventSubscriber>(configurator, eventSubscriber, serviceLifetime);
        }

        public static IEventSubscriberConfigurator WithEventSubscriber<TEventSubscriber>(this IHalSupportConfigurator configurator, Func<IServiceProvider, TEventSubscriber> eventSubscriberFactory, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
            where TEventSubscriber : class, IEventSubscriber
        {
            return new EventSubscriberConfigurator<TEventSubscriber>(configurator, eventSubscriberFactory, serviceLifetime);
        }

        public static ICommandBusConfigurator WithCommandBus<TCommandBus>(this IHalSupportConfigurator configurator, TCommandBus commandBus, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
            where TCommandBus : class, ICommandBus
        {
            return new CommandBusConfigurator<TCommandBus>(configurator, commandBus, serviceLifetime);
        }

        public static ICommandBusConfigurator WithCommandBus<TCommandBus>(this IHalSupportConfigurator configurator, Func<IServiceProvider, TCommandBus> commandBusFactory, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
            where TCommandBus : class, ICommandBus
        {
            return new CommandBusConfigurator<TCommandBus>(configurator, commandBusFactory, serviceLifetime);
        }

        public static ICommandSenderConfigurator WithCommandSender<TCommandSender>(this IHalSupportConfigurator configurator, TCommandSender commandSender, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
            where TCommandSender : class, ICommandSender
        {
            return new CommandSenderConfigurator<TCommandSender>(configurator, commandSender, serviceLifetime);
        }

        public static ICommandSenderConfigurator WithCommandSender<TCommandSender>(this IHalSupportConfigurator configurator, Func<IServiceProvider, TCommandSender> commandSenderFactory, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
            where TCommandSender : class, ICommandSender
        {
            return new CommandSenderConfigurator<TCommandSender>(configurator, commandSenderFactory, serviceLifetime);
        }

        public static ICommandSubscriberConfigurator WithCommandSubscriber<TCommandSubscriber>(this IHalSupportConfigurator configurator, TCommandSubscriber commandSubscriber, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
            where TCommandSubscriber : class, ICommandSubscriber
        {
            return new CommandSubscriberConfigurator<TCommandSubscriber>(configurator, commandSubscriber, serviceLifetime);
        }

        public static ICommandSubscriberConfigurator WithCommandSubscriber<TCommandSubscriber>(this IHalSupportConfigurator configurator, Func<IServiceProvider, TCommandSubscriber> commandSubscriberFactory, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
            where TCommandSubscriber : class, ICommandSubscriber
        {
            return new CommandSubscriberConfigurator<TCommandSubscriber>(configurator, commandSubscriberFactory, serviceLifetime);
        }

        #endregion

        #region ICommandBusConfigurator Extensions
        public static IEventBusConfigurator WithEventBus<TEventBus>(this ICommandBusConfigurator configurator, TEventBus eventBus, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
            where TEventBus : class, IEventBus
        {
            return new EventBusConfigurator<TEventBus>(configurator, eventBus, serviceLifetime);
        }

        public static IEventBusConfigurator WithEventBus<TEventBus>(this ICommandBusConfigurator configurator, Func<IServiceProvider, TEventBus> eventBusFactory, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
            where TEventBus : class, IEventBus
        {
            return new EventBusConfigurator<TEventBus>(configurator, eventBusFactory, serviceLifetime);
        }

        public static IEventPublisherConfigurator WithEventPublisher<TEventPublisher>(this ICommandBusConfigurator configurator, TEventPublisher eventPublisher, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
            where TEventPublisher : class, IEventPublisher
        {
            return new EventPublisherConfigurator<TEventPublisher>(configurator, eventPublisher, serviceLifetime);
        }

        public static IEventPublisherConfigurator WithEventPublisher<TEventPublisher>(this ICommandBusConfigurator configurator, Func<IServiceProvider, TEventPublisher> eventPublisherFactory, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
            where TEventPublisher : class, IEventPublisher
        {
            return new EventPublisherConfigurator<TEventPublisher>(configurator, eventPublisherFactory, serviceLifetime);
        }

        public static IEventSubscriberConfigurator WithEventSubscriber<TEventSubscriber>(this ICommandBusConfigurator configurator, TEventSubscriber eventSubscriber, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
            where TEventSubscriber : class, IEventSubscriber
        {
            return new EventSubscriberConfigurator<TEventSubscriber>(configurator, eventSubscriber, serviceLifetime);
        }

        public static IEventSubscriberConfigurator WithEventSubscriber<TEventSubscriber>(this ICommandBusConfigurator configurator, Func<IServiceProvider, TEventSubscriber> eventSubscriberFactory, ServiceLifetime serviceLifetime)
            where TEventSubscriber : class, IEventSubscriber
        {
            return new EventSubscriberConfigurator<TEventSubscriber>(configurator, eventSubscriberFactory, serviceLifetime);
        }

        public static IDataServiceConfigurator WithDataServiceSupport(this ICommandBusConfigurator configurator, DataServiceConfigurationOptions options)
        {
            return new DataServiceConfigurator(configurator, options);
        }
        #endregion

        #region ICommandSenderConfigurator Extensions
        public static ICommandSubscriberConfigurator WithCommandSubscriber<TCommandSubscriber>(this ICommandSenderConfigurator configurator, TCommandSubscriber commandSubscriber, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
            where TCommandSubscriber : class, ICommandSubscriber
        {
            return new CommandSubscriberConfigurator<TCommandSubscriber>(configurator, commandSubscriber, serviceLifetime);
        }

        public static ICommandSubscriberConfigurator WithCommandSubscriber<TCommandSubscriber>(this ICommandSenderConfigurator configurator, Func<IServiceProvider, TCommandSubscriber> commandSubscriberFactory, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
            where TCommandSubscriber : class, ICommandSubscriber
        {
            return new CommandSubscriberConfigurator<TCommandSubscriber>(configurator, commandSubscriberFactory, serviceLifetime);
        }

        public static IEventBusConfigurator WithEventBus<TEventBus>(this ICommandSenderConfigurator configurator, TEventBus eventBus, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
            where TEventBus : class, IEventBus
        {
            return new EventBusConfigurator<TEventBus>(configurator, eventBus, serviceLifetime);
        }

        public static IEventBusConfigurator WithEventBus<TEventBus>(this ICommandSenderConfigurator configurator, Func<IServiceProvider, TEventBus> eventBusFactory, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
            where TEventBus : class, IEventBus
        {
            return new EventBusConfigurator<TEventBus>(configurator, eventBusFactory, serviceLifetime);
        }

        public static IEventPublisherConfigurator WithEventPublisher<TEventPublisher>(this ICommandSenderConfigurator configurator, TEventPublisher eventPublisher, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
            where TEventPublisher : class, IEventPublisher
        {
            return new EventPublisherConfigurator<TEventPublisher>(configurator, eventPublisher, serviceLifetime);
        }

        public static IEventPublisherConfigurator WithEventPublisher<TEventPublisher>(this ICommandSenderConfigurator configurator, Func<IServiceProvider, TEventPublisher> eventPublisherFactory, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
            where TEventPublisher : class, IEventPublisher
        {
            return new EventPublisherConfigurator<TEventPublisher>(configurator, eventPublisherFactory, serviceLifetime);
        }

        public static IEventSubscriberConfigurator WithEventSubscriber<TEventSubscriber>(this ICommandSenderConfigurator configurator, TEventSubscriber eventSubscriber, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
            where TEventSubscriber : class, IEventSubscriber
        {
            return new EventSubscriberConfigurator<TEventSubscriber>(configurator, eventSubscriber, serviceLifetime);
        }

        public static IEventSubscriberConfigurator WithEventSubscriber<TEventSubscriber>(this ICommandSenderConfigurator configurator, Func<IServiceProvider, TEventSubscriber> eventSubscriberFactory, ServiceLifetime serviceLifetime)
            where TEventSubscriber : class, IEventSubscriber
        {
            return new EventSubscriberConfigurator<TEventSubscriber>(configurator, eventSubscriberFactory, serviceLifetime);
        }

        public static IDataServiceConfigurator WithDataServiceSupport(this ICommandSenderConfigurator configurator, DataServiceConfigurationOptions options)
        {
            return new DataServiceConfigurator(configurator, options);
        }
        #endregion

        #region ICommandSubscriberConfigurator Extensions
        public static IEventBusConfigurator WithEventBus<TEventBus>(this ICommandSubscriberConfigurator configurator, TEventBus eventBus, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
            where TEventBus : class, IEventBus
        {
            return new EventBusConfigurator<TEventBus>(configurator, eventBus, serviceLifetime);
        }

        public static IEventBusConfigurator WithEventBus<TEventBus>(this ICommandSubscriberConfigurator configurator, Func<IServiceProvider, TEventBus> eventBusFactory, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
            where TEventBus : class, IEventBus
        {
            return new EventBusConfigurator<TEventBus>(configurator, eventBusFactory, serviceLifetime);
        }

        public static IEventPublisherConfigurator WithEventPublisher<TEventPublisher>(this ICommandSubscriberConfigurator configurator, TEventPublisher eventPublisher, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
            where TEventPublisher : class, IEventPublisher
        {
            return new EventPublisherConfigurator<TEventPublisher>(configurator, eventPublisher, serviceLifetime);
        }

        public static IEventPublisherConfigurator WithEventPublisher<TEventPublisher>(this ICommandSubscriberConfigurator configurator, Func<IServiceProvider, TEventPublisher> eventPublisherFactory, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
            where TEventPublisher : class, IEventPublisher
        {
            return new EventPublisherConfigurator<TEventPublisher>(configurator, eventPublisherFactory, serviceLifetime);
        }

        public static IEventSubscriberConfigurator WithEventSubscriber<TEventSubscriber>(this ICommandSubscriberConfigurator configurator, TEventSubscriber eventSubscriber, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
            where TEventSubscriber : class, IEventSubscriber
        {
            return new EventSubscriberConfigurator<TEventSubscriber>(configurator, eventSubscriber, serviceLifetime);
        }

        public static IEventSubscriberConfigurator WithEventSubscriber<TEventSubscriber>(this ICommandSubscriberConfigurator configurator, Func<IServiceProvider, TEventSubscriber> eventSubscriberFactory, ServiceLifetime serviceLifetime)
            where TEventSubscriber : class, IEventSubscriber
        {
            return new EventSubscriberConfigurator<TEventSubscriber>(configurator, eventSubscriberFactory, serviceLifetime);
        }

        public static IDataServiceConfigurator WithDataServiceSupport(this ICommandSubscriberConfigurator configurator, DataServiceConfigurationOptions options)
        {
            return new DataServiceConfigurator(configurator, options);
        }
        #endregion

        #region ICommandHandlerConfigurator Extensions
        public static ICommandHandlerConfigurator AddCommandHandler<TCommandHandler>(this ICommandHandlerConfigurator configurator, TCommandHandler commandHandler, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
            where TCommandHandler : class, ICommandHandler
        {
            return new CommandHandlerConfigurator<TCommandHandler>(configurator, commandHandler, serviceLifetime);
        }

        public static ICommandHandlerConfigurator AddCommandHandler<TCommandHandler>(this ICommandHandlerConfigurator configurator, Func<IServiceProvider, TCommandHandler> commandHandlerFactory, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
            where TCommandHandler : class, ICommandHandler
        {
            return new CommandHandlerConfigurator<TCommandHandler>(configurator, commandHandlerFactory, serviceLifetime);
        }

        public static IEventBusConfigurator WithEventBus<TEventBus>(this ICommandHandlerConfigurator configurator, TEventBus eventBus, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
            where TEventBus : class, IEventBus
        {
            return new EventBusConfigurator<TEventBus>(configurator, eventBus, serviceLifetime);
        }

        public static IEventBusConfigurator WithEventBus<TEventBus>(this ICommandHandlerConfigurator configurator, Func<IServiceProvider, TEventBus> eventBusFactory, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
            where TEventBus : class, IEventBus
        {
            return new EventBusConfigurator<TEventBus>(configurator, eventBusFactory, serviceLifetime);
        }

        public static IEventPublisherConfigurator WithEventPublisher<TEventPublisher>(this ICommandHandlerConfigurator configurator, TEventPublisher eventPublisher, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
            where TEventPublisher : class, IEventPublisher
        {
            return new EventPublisherConfigurator<TEventPublisher>(configurator, eventPublisher, serviceLifetime);
        }

        public static IEventPublisherConfigurator WithEventPublisher<TEventPublisher>(this ICommandHandlerConfigurator configurator, Func<IServiceProvider, TEventPublisher> eventPublisherFactory, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
            where TEventPublisher : class, IEventPublisher
        {
            return new EventPublisherConfigurator<TEventPublisher>(configurator, eventPublisherFactory, serviceLifetime);
        }

        public static IEventSubscriberConfigurator WithEventSubscriber<TEventSubscriber>(this ICommandHandlerConfigurator configurator, TEventSubscriber eventSubscriber, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
            where TEventSubscriber : class, IEventSubscriber
        {
            return new EventSubscriberConfigurator<TEventSubscriber>(configurator, eventSubscriber, serviceLifetime);
        }

        public static IEventSubscriberConfigurator WithEventSubscriber<TEventSubscriber>(this ICommandHandlerConfigurator configurator, Func<IServiceProvider, TEventSubscriber> eventSubscriberFactory, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
            where TEventSubscriber : class, IEventSubscriber
        {
            return new EventSubscriberConfigurator<TEventSubscriber>(configurator, eventSubscriberFactory, serviceLifetime);
        }

        public static IDataServiceConfigurator WithDataServiceSupport(this ICommandHandlerConfigurator configurator, DataServiceConfigurationOptions options)
        {
            return new DataServiceConfigurator(configurator, options);
        }
        #endregion

        #region IEventBusConfigurator Extensions
        public static IDataServiceConfigurator WithDataServiceSupport(this IEventBusConfigurator configurator, DataServiceConfigurationOptions options)
        {
            return new DataServiceConfigurator(configurator, options);
        }
        #endregion

        #region IEventPublisherConfigurator Extensions
        public static IEventSubscriberConfigurator WithEventSubscriber<TEventSubscriber>(this IEventPublisherConfigurator configurator, TEventSubscriber eventSubscriber, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
            where TEventSubscriber : class, IEventSubscriber
        {
            return new EventSubscriberConfigurator<TEventSubscriber>(configurator, eventSubscriber, serviceLifetime);
        }

        public static IEventSubscriberConfigurator WithEventSubscriber<TEventSubscriber>(this IEventPublisherConfigurator configurator, Func<IServiceProvider, TEventSubscriber> eventSubscriberFactory, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
            where TEventSubscriber : class, IEventSubscriber
        {
            return new EventSubscriberConfigurator<TEventSubscriber>(configurator, eventSubscriberFactory, serviceLifetime);
        }

        public static IDataServiceConfigurator WithDataServiceSupport(this IEventPublisherConfigurator configurator, DataServiceConfigurationOptions options)
        {
            return new DataServiceConfigurator(configurator, options);
        }

        #endregion

        #region IEventSubscriberConfigurator Extensions

        public static IDataServiceConfigurator WithDataServiceSupport(this IEventSubscriberConfigurator configurator, DataServiceConfigurationOptions options)
        {
            return new DataServiceConfigurator(configurator, options);
        }
        #endregion

        #region IEventHandlerConfigurator Extensions
        public static IEventHandlerConfigurator AddEventHandler<TEventHandler>(this IEventHandlerConfigurator configurator, TEventHandler eventHandler, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
            where TEventHandler : class, IEventHandler
        {
            return new EventHandlerConfigurator<TEventHandler>(configurator, eventHandler, serviceLifetime);
        }

        public static IEventHandlerConfigurator AddEventHandler<TEventHandler>(this IEventHandlerConfigurator configurator, Func<IServiceProvider, TEventHandler> eventHandlerFactory, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
            where TEventHandler : class, IEventHandler
        {
            return new EventHandlerConfigurator<TEventHandler>(configurator, eventHandlerFactory, serviceLifetime);
        }

        public static IDataServiceConfigurator WithDataServiceSupport(this IEventHandlerConfigurator configurator, DataServiceConfigurationOptions options)
        {
            return new DataServiceConfigurator(configurator, options);
        }
        #endregion

        #region IApplicationBuilder Extensions
        public static IApplicationBuilder EnrichDataServiceExceptionResponse(this IApplicationBuilder applicationBuilder)
        {
            return applicationBuilder.UseMiddleware<DataServiceExceptionHandler>();
        }
        #endregion
    }
}
