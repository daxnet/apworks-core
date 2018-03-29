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

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Apworks.Messaging
{
    /// <summary>
    /// Represents that the implemented classes are message publishers that can
    /// publish the specified message to a message bus.
    /// </summary>
    public interface IMessagePublisher : IDisposable
    {
        /// <summary>
        /// Publishes the specified message to the message bus.
        /// </summary>
        /// <typeparam name="TMessage">The type of the message to be published.</typeparam>
        /// <param name="message">The message that is going to be published.</param>
        /// <param name="route">The routing of the message publication. In some of the message publisher implementation,
        /// this parameter can be ignored.</param>
        void Publish<TMessage>(TMessage message)
            where TMessage : IMessage;

        /// <summary>
        /// Publishes all the messages to the message bus.
        /// </summary>
        /// <param name="messages">The messages that is going to be published.</param>
        /// <param name="route">The routing of the message publication. In some of the message publisher implementation,
        /// this parameter can be ignored.</param>
        void PublishAll(IEnumerable<IMessage> messages);

        /// <summary>
        /// Publishes the specified message to the message bus asynchronously.
        /// </summary>
        /// <typeparam name="TMessage">The type of the message to be published.</typeparam>
        /// <param name="message">The message that is going to be published.</param>
        /// <param name="route">The routing of the message publication. In some of the message publisher implementation,
        /// this parameter can be ignored.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> instance that propagates notification that operations should be canceled.</param>
        /// <returns>The <see cref="Task"/> that executes the message publication.</returns>
        Task PublishAsync<TMessage>(TMessage message, CancellationToken cancellationToken = default(CancellationToken))
            where TMessage : IMessage;

        /// <summary>
        /// Publishes all the messages to the message bus asynchronously.
        /// </summary>
        /// <param name="messages">The messages that is going to be published.</param>
        /// <param name="route">The routing of the message publication. In some of the message publisher implementation,
        /// this parameter can be ignored.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> which propagates notification that operations should be canceled.</param>
        /// <returns>The <see cref="Task"/> that executes the message publication.</returns>
        Task PublishAllAsync(IEnumerable<IMessage> messages, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Represents the event that occurs when the message has been published.
        /// </summary>
        event EventHandler<MessagePublishedEventArgs> MessagePublished;
    }
}
