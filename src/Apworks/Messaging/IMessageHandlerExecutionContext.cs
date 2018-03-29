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
using System.Threading;
using System.Threading.Tasks;

namespace Apworks.Messaging
{
    /// <summary>
    /// Represents the message handler execution context, in which the message handlers are going
    /// to handle the received messages in a particular context, so that the dependent resources
    /// can be easily managed and maintained.
    /// </summary>
    public interface IMessageHandlerExecutionContext
    {
        /// <summary>
        /// Registers the message handler to the current execution context.
        /// </summary>
        /// <typeparam name="TMessage">The type of the message whose handler is going to be registered.</typeparam>
        /// <typeparam name="THandler">The type of the message handler that is going to be registered.</typeparam>
        void RegisterHandler<TMessage, THandler>()
            where TMessage : IMessage
            where THandler : IMessageHandler<TMessage>;

        /// <summary>
        /// Checks whether the message handler of the specified message type has been registered.
        /// </summary>
        /// <typeparam name="TMessage">The type of the message whose handler's existence should be checked.</typeparam>
        /// <typeparam name="THandler">The type of the message handler whose existence should be checked.</typeparam>
        /// <returns></returns>
        bool HandlerRegistered<TMessage, THandler>()
            where TMessage : IMessage
            where THandler : IMessageHandler<TMessage>;

        void RegisterHandler(Type messageType, Type handlerType);

        bool HandlerRegistered(Type messageType, Type handlerType);

        Task HandleMessageAsync(IMessage message, CancellationToken cancellationToken = default(CancellationToken));
    }
}
