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

using System;
using System.Threading;
using System.Threading.Tasks;

namespace Apworks.Messaging
{
    public interface IMessageHandler
    {
        bool Handle(IMessage message);

        bool CanHandle(Type messageType);

        Task<bool> HandleAsync(IMessage message, CancellationToken cancellationToken = default(CancellationToken));
    }

    /// <summary>
    /// Represents that the implemented classes are message handlers.
    /// </summary>
    /// <typeparam name="TMessage">The type of the message to be handled by current handler.</typeparam>
    public interface IMessageHandler<in TMessage> : IMessageHandler
        where TMessage : IMessage
    {
        /// <summary>
        /// Handles the specified message.
        /// </summary>
        /// <param name="message">The message to be handled.</param>
        /// <returns><c>true</c> if the message has been handled properly, otherwise, <c>false</c>.</returns>
        bool Handle(TMessage message);

        /// <summary>
        /// Handles the specified message asynchronously.
        /// </summary>
        /// <param name="message">The message to be handled.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> instance which propagates notification that operations should be canceled.</param>
        /// <returns><c>true</c> if the message has been handled properly, otherwise, <c>false</c>.</returns>
        Task<bool> HandleAsync(TMessage message, CancellationToken cancellationToken = default(CancellationToken));
    }
}
