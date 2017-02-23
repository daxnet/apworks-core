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

namespace Apworks.Integration.AspNetCore.Hal.Builders
{
    /// <summary>
    /// Represents that the implemented classes are the HAL resource
    /// builders that simply returned the built HAL resource.
    /// </summary>
    /// <seealso cref="Hal.Builders.IBuilder" />
    public interface IResourceBuilder : IBuilder { }

    /// <summary>
    /// Represents the HAL resource builder.
    /// </summary>
    /// <seealso cref="Hal.Builders.IResourceBuilder" />
    public sealed class ResourceBuilder : IResourceBuilder
    {
        #region Private Fields
        private readonly Resource resource = new Resource();
        #endregion

        #region Ctor        
        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceBuilder"/> class.
        /// </summary>
        public ResourceBuilder() { }
        #endregion

        #region Public Methods        
        /// <summary>
        /// Builds the <see cref="Resource"/> instance.
        /// </summary>
        /// <returns>The <see cref="Resource"/> instance to be built.</returns>
        public Resource Build()
        {
            return resource;
        }
        #endregion
    }
}
