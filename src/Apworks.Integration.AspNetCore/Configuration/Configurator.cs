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

using Microsoft.Extensions.DependencyInjection;

namespace Apworks.Integration.AspNetCore.Configuration
{
    /// <summary>
    /// Represents that the implemented classes are the configurators that can
    /// register the specified service implementations against their service contracts
    /// to the <see cref="IServiceCollection"/> instance.
    /// </summary>
    public interface IConfigurator
    {
        /// <summary>
        /// Configures and registers the service implementations against
        /// their service contracts to the <see cref="IServiceCollection"/> instance and returns
        /// the <see cref="IServiceCollection"/> instance that has the services registered.
        /// </summary>
        /// <returns>The <see cref="IServiceCollection"/> instance that contains the registration of
        /// the services.</returns>
        IServiceCollection Configure();
    }

    /// <summary>
    /// Represents the base class for all the configurators.
    /// </summary>
    /// <seealso cref="Apworks.Integration.AspNetCore.Configuration.IConfigurator" />
    internal abstract class Configurator : IConfigurator
    {
        #region Private Fields
        private readonly IConfigurator context;
        #endregion

        #region Ctor        
        /// <summary>
        /// Initializes a new instance of the <see cref="Configurator"/> class.
        /// </summary>
        /// <param name="context">The context which already had a series of services
        /// registered and provides the <see cref="IServiceCollection"/> instance that
        /// can be used by the current configurator.</param>
        protected Configurator(IConfigurator context)
        {
            this.context = context;
        }
        #endregion

        #region Protected Methods        
        /// <summary>
        /// Performs the configuration of the service implementations and register them
        /// against their service contracts to the given <see cref="IServiceCollection"/> instance.
        /// </summary>
        /// <param name="context">The <see cref="IServiceCollection"/> instance to which the services should
        /// be registered.</param>
        /// <returns>The <see cref="IServiceCollection"/> instance that contains the registration of
        /// the services.</returns>
        protected abstract IServiceCollection DoConfigure(IServiceCollection context);
        #endregion

        #region Public Methods        
        /// <summary>
        /// Configures and registers the service implementations against
        /// their service contracts to the <see cref="IServiceCollection" /> instance and returns
        /// the <see cref="IServiceCollection" /> instance that has the services registered.
        /// </summary>
        /// <returns>The <see cref="IServiceCollection"/> instance that contains the registration of
        /// the services.</returns>
        public IServiceCollection Configure()
        {
            var serviceCollection = this.context.Configure();
            return DoConfigure(serviceCollection);
        }
        #endregion
    }
}
