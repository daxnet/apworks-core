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

using Apworks.Integration.AspNetCore.Hal;
using Hal.Builders;
using Microsoft.AspNetCore.Http.Extensions;
using System;
using System.Collections.Generic;

namespace Apworks.Integration.AspNetCore.DataServices
{
    /// <summary>
    /// Represents the HAL build configuration that configures the HAL builder
    /// factory for data services.
    /// </summary>
    public class DataServiceHalBuildConfiguration : PagedResultHalBuildConfiguration
    {
        #region Ctor
        /// <summary>
        /// Initializes a new instance of the <see cref="DataServiceHalBuildConfiguration"/> class.
        /// </summary>
        public DataServiceHalBuildConfiguration()
            : base("*.Get(int, int, *, *)")
        {
            this.RegisterHalBuilderFactoryForGet();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataServiceHalBuildConfiguration"/> class.
        /// </summary>
        /// <param name="halBuilderFactories">The hal builder factories.</param>
        public DataServiceHalBuildConfiguration(IEnumerable<KeyValuePair<ControllerActionSignature, Func<HalBuildContext, IBuilder>>> halBuilderFactories)
            : this()
        {
            foreach (var factory in halBuilderFactories)
            {
                this.RegisterHalBuilderFactory(factory.Key, factory.Value);
            }
        }
        #endregion

        #region Protected Methods
        /// <summary>
        /// Registers the HAL builder factory for the HTTP GET method that returns a particular aggregate
        /// with a given aggregate root key.
        /// </summary>
        protected virtual void RegisterHalBuilderFactoryForGet()
        {
            this.RegisterHalBuilderFactory("*.Get(*)", context =>
            {
                return new ResourceBuilder()
                    .WithState(null)
                    .AddSelfLink().WithLinkItem(context.HttpContext.Request.GetDisplayUrl())
                    .AddEmbedded(context.State.GetType().Name.ToLower())
                    .Resource(new ResourceBuilder().WithState(context.State));
            });
        }

        #endregion
    }
}
