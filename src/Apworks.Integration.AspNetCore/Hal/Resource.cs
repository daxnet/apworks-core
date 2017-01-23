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

using Apworks.Integration.AspNetCore.Hal.Converters;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Apworks.Integration.AspNetCore.Hal
{
    /// <summary>
    /// Represents a resource in HAL.
    /// </summary>
    /// <seealso cref="Hal.IResource" />
    public sealed class Resource : IResource
    {
        #region Private Fields
        private static readonly List<JsonConverter> converters = new List<JsonConverter>
        {
            new LinkItemConverter(), new LinkItemCollectionConverter(), new LinkConverter(),
            new LinkCollectionConverter(), new ResourceConverter()
        };

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="Resource"/> class.
        /// </summary>
        public Resource() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Resource"/> class.
        /// </summary>
        /// <param name="state">The state of the resource.</param>
        public Resource(object state)
        {
            this.State = state;
        }

        #region Public Properties        
        /// <summary>
        /// Gets the embedded resources.
        /// </summary>
        /// <value>
        /// The embedded resources.
        /// </value>
        public EmbeddedResourceCollection EmbeddedResources { get; set; }

        /// <summary>
        /// Gets or sets the links.
        /// </summary>
        /// <value>
        /// The links.
        /// </value>
        public LinkCollection Links { get; set; }

        /// <summary>
        /// Gets or sets the state of the resource, usually it is the object
        /// that holds the domain information.
        /// </summary>
        /// <value>
        /// The state of the resource.
        /// </value>
        public object State { get; set; }
        #endregion

        #region Public Methods

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            var settings = new JsonSerializerSettings
            {
                Converters = converters,
                NullValueHandling = NullValueHandling.Ignore,
                Formatting = Formatting.Indented
            };

            return JsonConvert.SerializeObject(this, settings);
        }
        #endregion
    }
}
