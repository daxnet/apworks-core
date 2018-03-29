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

using Apworks.Messaging;

namespace Apworks.Events
{
    /// <summary>
    /// Represents that the implemented classes are events.
    /// </summary>
    public interface IEvent : IMessage
    {
        /// <summary>
        /// Gets the intent of the current event. Usually it is the simplified name
        /// of the <c>ClrType</c>.
        /// </summary>
        /// <value>
        /// The intent of the current event.
        /// </value>
        string GetEventIntent();

        /// <summary>
        /// Gets the CLR type (a.k.a. assembly qualified type) of the originator of the current event.
        /// </summary>
        /// <returns>The CLR type of the originator.</returns>
        string GetOriginatorClrType();

        /// <summary>
        /// Gets the originator identifier.
        /// </summary>
        /// <returns></returns>
        string GetOriginatorIdentifier();

        /// <summary>
        /// Converts the current <see cref="IEvent"/> instance to a <see cref="EventDescriptor"/>.
        /// </summary>
        /// <returns>The event descriptor.</returns>
        EventDescriptor ToDescriptor();
    }
}
