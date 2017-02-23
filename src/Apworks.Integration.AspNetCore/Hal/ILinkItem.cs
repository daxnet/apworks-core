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

namespace Apworks.Integration.AspNetCore.Hal
{
    /// <summary>
    /// Represents that the implemented classes are link items.
    /// </summary>
    /// <remarks>
    /// For more information about the definition of each property, please refer to: https://tools.ietf.org/html/draft-kelly-json-hal-08.
    /// </remarks>
    public interface ILinkItem
    {
        /// <summary>
        /// Gets or sets the name attribute of a link item. This value is optional.
        /// </summary>
        /// <remarks>
        /// Its value MAY be used as a secondary key for selecting Link Objects
        /// which share the same relation type.
        /// </remarks>
        string Name { get; set; }

        /// <summary>
        /// Gets or sets the href attribute of a link item. This value is required.
        /// </summary>
        /// <remarks>
        /// Its value is either a URI [RFC3986] or a URI Template [RFC6570].
        /// If the value is a URI Template then the <see cref="Link"/> Object SHOULD have a
        /// "templated" attribute whose value is true.
        /// </remarks>
        string Href { get; set; }

        /// <summary>
        /// Gets or sets a <see cref="bool"/> value which indicates if the <c>Href</c> property
        /// is a URI template. This value is optional.
        /// </summary>
        /// <remarks>
        /// Its value SHOULD be considered false if it is undefined or any other
        /// value than true.
        /// </remarks>
        bool? Templated { get; set; }

        /// <summary>
        /// Gets or sets the media type expected when dereferencing the target source. This value
        /// is optional.
        /// </summary>
        string Type { get; set; }

        /// <summary>
        /// Gets or sets a URL which provides further information about the deprecation. This value is
        /// optional.
        /// </summary>
        /// <remarks>
        /// Its presence indicates that the link is to be deprecated (i.e.
        /// removed) at a future date.Its value is a URL that SHOULD provide
        /// further information about the deprecation.
        /// A client SHOULD provide some notification (for example, by logging a
        /// warning message) whenever it traverses over a link that has this
        /// property.The notification SHOULD include the deprecation property's
        /// value so that a client manitainer can easily find information about
        /// the deprecation.
        /// </remarks>
        string Deprecation { get; set; }

        /// <summary>
        /// Gets or sets the URI that hints about the profile of the target resource. This
        /// value is optional.
        /// </summary>
        string Profile { get; set; }

        /// <summary>
        /// Gets or sets a <see cref="string"/> value which is intended for labelling
        /// the link with a human-readable identifier. This value is optional.
        /// </summary>
        string Title { get; set; }

        /// <summary>
        /// Gets or sets a <see cref="string"/> value which is intending for indicating
        /// the language of the target resource.
        /// </summary>
        string Hreflang { get; set; }

        /// <summary>
        /// Gets a list of additional properties assigned to the current link item.
        /// </summary>
        IEnumerable<KeyValuePair<string, object>> Properties { get; }

        /// <summary>
        /// Adds a property to the additional properties collection.
        /// </summary>
        /// <param name="name">The name of the property.</param>
        /// <param name="value">The value of the property.</param>
        void AddProperty(string name, object value);

        /// <summary>
        /// Removes all the properties from the properties collection.
        /// </summary>
        void ClearProperties();

        /// <summary>
        /// Gets the property value by using the specified name.
        /// </summary>
        /// <param name="name">The name of the property which the property value should be returned.</param>
        /// <returns>The value of the property.</returns>
        object GetProperty(string name);
    }
}
