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

using System.Threading;
using System.Threading.Tasks;

namespace Apworks.Messaging
{
    /// <summary>
    /// Represents that the implemented classes are message serializers.
    /// </summary>
    public interface IMessageSerializer
    {
        /// <summary>
        /// Serializes the specified message into a byte array.
        /// </summary>
        /// <typeparam name="TMessage">The type of the message to be serialized.</typeparam>
        /// <param name="message">The message that is going to be serialized.</param>
        /// <returns>A byte array that contains the serialized data.</returns>
        byte[] Serialize<TMessage>(TMessage message)
            where TMessage : IMessage;

        /// <summary>
        /// Serializes the specified message into a byte array asynchronously.
        /// </summary>
        /// <typeparam name="TMessage">The type of the message to be serialized.</typeparam>
        /// <param name="message">The message that is going to be serialized.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> instance that propagates notification that operations should be canceled.</param>
        /// <returns>The <see cref="Task"/> that when finished, returns the serialized data.</returns>
        Task<byte[]> SerializeAsync<TMessage>(TMessage message, CancellationToken cancellationToken = default(CancellationToken))
            where TMessage : IMessage;

        /// <summary>
        /// Deserializes the message from the specified <see cref="byte"/> array.
        /// </summary>
        /// <param name="value">The <see cref="byte"/> array which contains the message data.</param>
        /// <returns>The deserialized message.</returns>
        IMessage Deserialize(byte[] value);

        /// <summary>
        /// Deserializes a message from the specified <see cref="byte"/> array asynchronously.
        /// </summary>
        /// <param name="value">The byte array that contains the serialized data.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> instance that propagates notification that operations should be canceled.</param>
        /// <returns>The deserialized message.</returns>
        Task<IMessage> DeserializeAsync(byte[] value, CancellationToken cancellationToken = default(CancellationToken));
    }
}
