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
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Apworks.Messaging.Simple
{
    /// <summary>
    /// Represents the message bus that uses a dictionary data structure as the message queue
    /// infrastructure.
    /// </summary>
    /// <seealso cref="Apworks.DisposableObject" />
    /// <seealso cref="Apworks.Messaging.IMessageBus" />
    public class MessageBus : DisposableObject, IMessageBus
    {
        private readonly MessageQueue messageQueue = new MessageQueue();
        private bool subscribed = false;

        /// <summary>
        /// Occurs when a message has been published to the message bus.
        /// </summary>
        public event EventHandler<MessagePublishedEventArgs> MessagePublished;

        /// <summary>
        /// Occurs when a message has been received.
        /// </summary>
        public event EventHandler<MessageReceivedEventArgs> MessageReceived;

        /// <summary>
        /// Occurs when the message has been acknowledged.
        /// </summary>
        public event EventHandler<MessageProcessedEventArgs> MessageAcknowledged;

        /// <summary>
        /// Publishes the specified message.
        /// </summary>
        /// <typeparam name="TMessage">The type of the message.</typeparam>
        /// <param name="message">The message.</param>
        /// <param name="route">The route.</param>
        public void Publish<TMessage>(TMessage message, string route = null) where TMessage : IMessage
        {
            messageQueue.PushMessage(message);
            this.OnMessagePublished(new MessagePublishedEventArgs(message));
        }

        /// <summary>
        /// Publishes the messages.
        /// </summary>
        /// <param name="messages">The messages to be published.</param>
        /// <param name="route">The routing for the publishing message. In some of the message publisher implementation,
        /// the routing can be ignored.</param>
        public void PublishAll(IEnumerable<IMessage> messages, string route = null) => messages.ToList().ForEach(msg => Publish(msg));

        /// <summary>
        /// publishes the specified message asynchronously.
        /// </summary>
        /// <typeparam name="TMessage">The type of the message to be published.</typeparam>
        /// <param name="message">The message that is going to be published.</param>
        /// <param name="route">The routing for the publishing message. In some of the message publisher implementation,
        /// the routing can be ignored.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> instance that propagates notification that operations should be canceled.</param>
        /// <returns></returns>
        public Task PublishAsync<TMessage>(TMessage message, string route = null, CancellationToken cancellationToken = default(CancellationToken)) 
            where TMessage : IMessage => Task.Factory.StartNew(() => Publish(message), cancellationToken);

        /// <summary>
        /// Publishes the messages asynchronously.
        /// </summary>
        /// <param name="messages">A series of messages to be published.</param>
        /// <param name="route">The routing for the publishing message. In some of the message publisher implementation,
        /// the routing can be ignored.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> which propagates notification that operations should be canceled.</param>
        /// <returns></returns>
        public Task PublishAllAsync(IEnumerable<IMessage> messages, string route = null, CancellationToken cancellationToken = default(CancellationToken)) => Task.Factory.StartNew(() => PublishAll(messages), cancellationToken);

        /// <summary>
        /// Subscribes to the underlying messaging infrastructure.
        /// </summary>
        /// <param name="route">The routing that the current subscriber will use. In some of the message publisher implementation,
        /// the routing can be ignored.</param>
        public void Subscribe(string route = null)
        {
            if (!subscribed)
            {
                messageQueue.MessagePushed += (s, e) =>
                {
                    var message = ((MessageQueue)s).PopMessage();
                    this.OnMessageReceived(new MessageReceivedEventArgs(message));
                    this.OnMessageAcknowledged(new MessageProcessedEventArgs(message));
                };
                subscribed = true;
            }
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing) { }

        private void OnMessagePublished(MessagePublishedEventArgs e)
        {
            this.MessagePublished?.Invoke(this, e);
        }

        private void OnMessageReceived(MessageReceivedEventArgs e)
        {
            this.MessageReceived?.Invoke(this, e);
        }

        private void OnMessageAcknowledged(MessageProcessedEventArgs e)
        {
            this.MessageAcknowledged?.Invoke(this, e);
        }
    }
}
