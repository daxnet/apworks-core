﻿// ==================================================================================================================                                                                                          
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
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using Apworks.Utilities;

namespace Apworks.Messaging.Simple
{
    /// <summary>
    /// Represents the message bus that uses a dictionary data structure as the message queue
    /// infrastructure.
    /// </summary>
    /// <seealso cref="Apworks.DisposableObject" />
    /// <seealso cref="Apworks.Messaging.IMessageBus" />
    public class SimpleMessageBus : MessageBus
    {
        private readonly MessageQueue messageQueue;

        public SimpleMessageBus(IMessageSerializer messageSerializer,
            IMessageHandlerExecutionContext messageHandlerExecutionContext)
            : base(messageSerializer, messageHandlerExecutionContext)
        {
            this.messageQueue = new MessageQueue(this.MessageSerializer);
            this.messageQueue.MessagePushed += (s, e) =>
            {
                var message = ((MessageQueue)s).PopMessage();
                this.OnMessageReceived(new MessageReceivedEventArgs(message, this.MessageSerializer));
                this.MessageHandlerExecutionContext.HandleMessageAsync(message).GetAwaiter().GetResult();
                this.OnMessageAcknowledged(new MessageAcknowledgedEventArgs(message, this.MessageSerializer));
            };
        }

        protected override void DoPublish<TMessage>(TMessage message)
        {
            messageQueue.PushMessage(message);
        }

        protected override Task DoPublishAsync<TMessage>(TMessage message, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.Factory.StartNew(() => DoPublish(message), cancellationToken);
        }

        public override void Subscribe<TMessage, TMessageHandler>()
        {
            this.MessageHandlerExecutionContext.RegisterHandler<TMessage, TMessageHandler>();
        }
    }
}
