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

using System;
using Microsoft.Extensions.DependencyInjection;

namespace Apworks.Integration.AspNetCore.Configuration
{
    /// <summary>
    /// Represents the service register configurator which registers the specified service implementation
    /// to the service collection.
    /// </summary>
    /// <typeparam name="TService">The type of the service.</typeparam>
    /// <typeparam name="TImplementation">The type of the implementation.</typeparam>
    /// <seealso cref="Apworks.Integration.AspNetCore.Configuration.Configurator" />
    internal abstract class ServiceRegisterConfigurator<TService, TImplementation> : Configurator
        where TService : class
        where TImplementation : class, TService
    {
        #region Private Fields
        private readonly TImplementation implementation;
        private readonly Func<IServiceProvider, TImplementation> implementationFactory;
        private readonly ServiceLifetime serviceLifetime;
        #endregion

        #region Ctor        
        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceRegisterConfigurator{TService, TImplementation}"/> class.
        /// </summary>
        /// <param name="context">The context that can register the specified service implementations against their service contracts
        /// to the <see cref="IServiceCollection"/> instance.</param>
        /// <param name="implementation">The implementation of the service that is going to be registered.</param>
        /// <param name="serviceLifetime">The service lifetime.</param>
        protected ServiceRegisterConfigurator(IConfigurator context, TImplementation implementation, ServiceLifetime serviceLifetime) : base(context)
        {
            this.implementation = implementation;
            this.serviceLifetime = serviceLifetime;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceRegisterConfigurator{TService, TImplementation}"/> class.
        /// </summary>
        /// <param name="context">The context that can register the specified service implementations against their service contracts
        /// to the <see cref="IServiceCollection"/> instance.</param>
        /// <param name="implementationFactory">The implementation factory that creates the implementation of the service that is going
        /// to be registered.</param>
        /// <param name="serviceLifetime">The service lifetime.</param>
        protected ServiceRegisterConfigurator(IConfigurator context, Func<IServiceProvider, TImplementation> implementationFactory, ServiceLifetime serviceLifetime)
            : base(context)
        {
            this.implementationFactory = implementationFactory;
            this.serviceLifetime = serviceLifetime;
        }
        #endregion

        #region Protected Methods        
        /// <summary>
        /// Performs the configuration of the service implementations and register them
        /// against their service contracts to the given <see cref="T:Microsoft.Extensions.DependencyInjection.IServiceCollection" /> instance.
        /// </summary>
        /// <param name="context">The <see cref="T:Microsoft.Extensions.DependencyInjection.IServiceCollection" /> instance to which the services should
        /// be registered.</param>
        /// <returns>
        /// The <see cref="T:Microsoft.Extensions.DependencyInjection.IServiceCollection" /> instance that contains the registration of
        /// the services.
        /// </returns>
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
        #endregion
    }
}
