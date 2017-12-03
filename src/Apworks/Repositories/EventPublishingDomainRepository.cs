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

using Apworks.Events;
using Apworks.Messaging;

namespace Apworks.Repositories
{
    /// <summary>
    /// Represents the domain repository that can publish the uncommitted domain events in an aggregate
    /// to the message bus.
    /// </summary>
    public abstract class EventPublishingDomainRepository : DomainRepository
    {
        #region Private Fields
        private readonly IEventPublisher publisher;
        private bool disposed = false;
        #endregion

        #region Protected Properties        
        /// <summary>
        /// Gets the instance of the event publisher.
        /// </summary>
        /// <value>
        /// The event publisher which publishes the events.
        /// </value>
        protected IEventPublisher Publisher => this.publisher;
        #endregion

        #region Ctor        
        /// <summary>
        /// Initializes a new instance of the <see cref="EventPublishingDomainRepository"/> class.
        /// </summary>
        /// <param name="publisher">The event publisher which publishes the events.</param>
        protected EventPublishingDomainRepository(IEventPublisher publisher)
        {
            this.publisher = publisher;
            this.publisher.MessagePublished += OnMessagePublished;
        }
        #endregion

        #region Protected Methods        
        /// <summary>
        /// Invokes when the event has been published.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="MessagePublishedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnMessagePublished(object sender, MessagePublishedEventArgs e) { }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    this.publisher.Dispose();
                }

                disposed = true;
                base.Dispose(disposing);
            }
        }
        #endregion
    }
}
