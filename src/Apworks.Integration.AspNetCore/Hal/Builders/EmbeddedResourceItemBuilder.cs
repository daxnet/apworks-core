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

namespace Apworks.Integration.AspNetCore.Hal.Builders
{
    /// <summary>
    /// Represents that the implemented classes are HAL resource builders that
    /// will add an embedded resource to the embedded resource collection of
    /// the building HAL resource.
    /// </summary>
    /// <seealso cref="Hal.Builders.IBuilder" />
    public interface IEmbeddedResourceItemBuilder : IBuilder
    {
        /// <summary>
        /// Gets the name of the embedded resource collection.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        string Name { get; }
    }

    /// <summary>
    /// Represents an internal implementation of the <see cref="IEmbeddedResourceItemBuilder"/> class.
    /// </summary>
    /// <seealso cref="Hal.Builders.Builder" />
    /// <seealso cref="Hal.Builders.IEmbeddedResourceItemBuilder" />
    internal sealed class EmbeddedResourceItemBuilder : Builder, IEmbeddedResourceItemBuilder
    {
        #region Private Fields
        private readonly string name;
        private readonly IBuilder resourceBuilder;
        #endregion

        #region Ctor        
        /// <summary>
        /// Initializes a new instance of the <see cref="EmbeddedResourceItemBuilder"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="name">The name of the embedded resource collection.</param>
        /// <param name="resourceBuilder">The resource builder that will build the embedded resource.</param>
        public EmbeddedResourceItemBuilder(IBuilder context, string name, IBuilder resourceBuilder) : base(context)
        {
            this.name = name;
            this.resourceBuilder = resourceBuilder;
        }
        #endregion

        #region Public Properties        
        /// <summary>
        /// Gets the name of the embedded resource collection.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name => this.name;
        #endregion

        #region Protected Methods        
        /// <summary>
        /// Builds the <see cref="Resource" /> instance.
        /// </summary>
        /// <param name="resource"></param>
        /// <returns>
        /// The <see cref="Resource" /> instance to be built.
        /// </returns>
        protected override Resource DoBuild(Resource resource)
        {
            var embeddedResource = resource.EmbeddedResources.First(x => x.Name.Equals(this.name));
            if (embeddedResource == null)
            {
                embeddedResource = new EmbeddedResource
                {
                    Name = this.name,
                    Resources = new ResourceCollection
                    {
                        this.resourceBuilder.Build()
                    }
                };
                resource.EmbeddedResources.Add(embeddedResource);
            }
            else
            {
                embeddedResource.Resources.Add(this.resourceBuilder.Build());
            }

            return resource;
        }
        #endregion
    }
}
