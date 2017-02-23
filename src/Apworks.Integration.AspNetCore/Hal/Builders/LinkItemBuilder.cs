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
using System.Collections.Generic;
using System.Linq;

namespace Apworks.Integration.AspNetCore.Hal.Builders
{
    /// <summary>
    /// Represents that the implemented classes are the builders
    /// that are responsible for adding the <see cref="ILinkItem"/>
    /// objects to the HAL resource.
    /// </summary>
    /// <seealso cref="Hal.Builders.IBuilder" />
    public interface ILinkItemBuilder : IBuilder
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
    /// Represents an internal implementation of <see cref="ILinkItemBuilder"/>.
    /// </summary>
    /// <seealso cref="Hal.Builders.Builder" />
    /// <seealso cref="Hal.Builders.ILinkItemBuilder" />
    internal sealed class LinkItemBuilder : Builder, ILinkItemBuilder
    {
        #region Private Fields
        private readonly string rel;
        private readonly string href;
        private readonly string name;
        private readonly bool? templated;
        private readonly string type;
        private readonly string deprecation;
        private readonly string profile;
        private readonly string title;
        private readonly string hreflang;
        private readonly bool enforcingArrayConverting;
        private readonly IDictionary<string, object> additionalProperties;
        #endregion

        #region Ctor        
        /// <summary>
        /// Initializes a new instance of the <see cref="LinkItemBuilder"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="rel">The relation of the resource location.</param>
        /// <param name="href">The href attribute of a link item.</param>
        /// <param name="name">The name attribute of a link item.</param>
        /// <param name="templated">The <see cref="bool"/> value which indicates if the <c>Href</c> property
        /// is a URI template.</param>
        /// <param name="type">The media type expected when dereferencing the target source.</param>
        /// <param name="deprecation">The URL which provides further information about the deprecation.</param>
        /// <param name="profile">The URI that hints about the profile of the target resource.</param>
        /// <param name="title">The <see cref="string"/> value which is intended for labelling
        /// the link with a human-readable identifier.</param>
        /// <param name="hreflang">The <see cref="string"/> value which is intending for indicating
        /// the language of the target resource.</param>
        /// <param name="enforcingArrayConverting">The value indicating whether the generated Json representation should be in an array
        /// format, even if the number of items is only one.</param>
        /// <param name="additionalProperties">The additional properties.</param>
        public LinkItemBuilder(IBuilder context, string rel, string href,
            string name = null, bool? templated = null, string type = null,
            string deprecation = null, string profile = null, string title = null,
            string hreflang = null, bool enforcingArrayConverting = false, 
            IDictionary<string, object> additionalProperties = null) : base(context)
        {
            this.rel = rel;
            this.href = href;
            this.name = name;
            this.templated = templated;
            this.type = type;
            this.deprecation = deprecation;
            this.profile = profile;
            this.title = title;
            this.hreflang = hreflang;
            this.enforcingArrayConverting = enforcingArrayConverting;
            this.additionalProperties = additionalProperties;
        }
        #endregion

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
            var link = resource.Links.FirstOrDefault(x => x.Rel.Equals(this.rel));
            if (link == null)
            {
                link = new Link(this.rel);
                resource.Links.Add(link);
            }

            if (link.Items == null)
            {
                link.Items = new LinkItemCollection(this.enforcingArrayConverting);
            }

            var linkItem = new LinkItem(this.href)
            {
                Deprecation = this.deprecation,
                Hreflang = this.hreflang,
                Name = this.name,
                Profile = this.profile,
                Templated = this.templated,
                Title = this.title,
                Type = this.type
            };

            if (this.additionalProperties!= null && this.additionalProperties.Count > 0)
            {
                foreach(var property in this.additionalProperties)
                {
                    linkItem.AddProperty(property.Key, property.Value);
                }
            }

            link.Items.Add(linkItem);

            return resource;
        }
        #endregion
    }
}
