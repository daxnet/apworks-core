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

using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Apworks.Repositories;
using Apworks.Integration.AspNetCore.Hal;

namespace Apworks.Integration.AspNetCore.Configuration
{
    /// <summary>
    /// Represents that the implemented classes are data service configurators that can register a bunch of service instances
    /// that are required or related to the ASP.NET Core data service integration.
    /// </summary>
    /// <seealso cref="Apworks.Integration.AspNetCore.Configuration.IConfigurator" />
    public interface IDataServiceConfigurator : IConfigurator
    { }

    /// <summary>
    /// Represents a default implementation of <see cref="IDataServiceConfigurator"/> interface.
    /// </summary>
    /// <seealso cref="Apworks.Integration.AspNetCore.Configuration.Configurator" />
    /// <seealso cref="Apworks.Integration.AspNetCore.Configuration.IDataServiceConfigurator" />
    internal sealed class DataServiceConfigurator : Configurator, IDataServiceConfigurator
    {
        #region Private Fields
        private readonly DataServiceConfigurationOptions options;
        #endregion

        #region Ctor        
        /// <summary>
        /// Initializes a new instance of the <see cref="DataServiceConfigurator"/> class.
        /// </summary>
        /// <param name="context">The context which already had a series of services
        /// registered and provides the <see cref="IServiceCollection"/> instance that
        /// can be used by the current configurator.</param>
        /// <param name="options">The <see cref="DataServiceConfigurationOptions"/> instance which holds the
        /// data that contains the options for the data service configuration.</param>
        public DataServiceConfigurator(IConfigurator context, DataServiceConfigurationOptions options) : base(context)
        {
            this.options = options;
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
            // Registers the Repository Context, which is a required component to the ASP.NET Core data service integration.
            switch(this.options.RepositoryContextLifetime)
            {
                case ServiceLifetime.Scoped:
                    context.AddScoped<IRepositoryContext>(this.options.RepositoryContextFactory);
                    break;
                case ServiceLifetime.Singleton:
                    context.AddSingleton<IRepositoryContext>(this.options.RepositoryContextFactory);
                    break;
                case ServiceLifetime.Transient:
                    context.AddTransient<IRepositoryContext>(this.options.RepositoryContextFactory);
                    break;
            }

            // Registers the HAL build configuration component, if the UseHalSupport property evaluates true.
            if (this.options.UseHalSupport)
            {
                context.AddScoped<IHalBuildConfiguration>(this.options.HalBuildConfigurationFactory);
            }

            // Registers additional services, this can also be done from the IServiceCollection interface in the Startup
            // method of ASP.NET Core.
            if (this.options.ServiceFactoryRegistrations.Count() > 0)
            {
                foreach(var item in this.options.ServiceFactoryRegistrations)
                { 
                    switch(item.Item3)
                    {
                        case ServiceLifetime.Scoped:
                            context.AddScoped(item.Item1, item.Item2);
                            break;
                        case ServiceLifetime.Singleton:
                            context.AddSingleton(item.Item1, item.Item2);
                            break;
                        case ServiceLifetime.Transient:
                            context.AddTransient(item.Item1, item.Item2);
                            break;
                    }
                }
            }

            return context;
        }
        #endregion
    }
}
