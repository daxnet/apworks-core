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
    /// Represents that the implemented classes are HAL resource builders
    /// that are responsible for adding the <see cref="ILink"/> instance
    /// to the HAL resource.
    /// </summary>
    /// <seealso cref="Hal.Builders.IBuilder" />
    public interface ILinkBuilder : IBuilder
    {
        /// <summary>
        /// Gets the relation of the resource location.
        /// </summary>
        /// <value>
        /// The relation of the resource location.
        /// </value>
        string Rel { get; }

        /// <summary>
        /// Gets a value indicating whether the generated Json representation should be in an array
        /// format, even if the number of items is only one.
        /// </summary>
        /// <value>
        /// <c>true</c> if the generated Json representation should be in an array
        /// format; otherwise, <c>false</c>.
        /// </value>
        bool EnforcingArrayConverting { get; }
    }

    /// <summary>
    /// Represents an internal implementation of <see cref="ILinkBuilder"/> interface.
    /// </summary>
    /// <seealso cref="Hal.Builders.Builder" />
    /// <seealso cref="Hal.Builders.ILinkBuilder" />
    internal sealed class LinkBuilder : Builder, ILinkBuilder
    {
        #region Private Fields
        private readonly string rel;
        private readonly bool enforcingArrayConverting;
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="LinkBuilder"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="rel">The relation of the resource location.</param>
        /// <param name="enforcingArrayConverting">The value indicating whether the generated Json representation should be in an array
        /// format, even if the number of items is only one.</param>
        public LinkBuilder(IBuilder context, string rel, bool enforcingArrayConverting) 
            : base(context)
        {
            this.rel = rel;
            this.enforcingArrayConverting = enforcingArrayConverting;
        }

        #region Public Properties
        /// <summary>
        /// Gets the relation of the resource location.
        /// </summary>
        /// <value>
        /// The relation of the resource location.
        /// </value>
        public string Rel => this.rel;

        /// <summary>
        /// Gets a value indicating whether the generated Json representation should be in an array
        /// format, even if the number of items is only one.
        /// </summary>
        /// <value>
        /// <c>true</c> if the generated Json representation should be in an array
        /// format; otherwise, <c>false</c>.
        /// </value>
        public bool EnforcingArrayConverting => this.enforcingArrayConverting;
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
            if (resource.Links == null)
            {
                resource.Links = new LinkCollection();
            }

            var link = resource.Links.FirstOrDefault(x => x.Rel.Equals(this.rel));
            if (link == null)
            {
                resource.Links.Add(new Link(this.rel));
            }

            return resource;
        }
        #endregion
    }
}
