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

namespace Apworks
{
    /// <summary>
    /// Represents the base class for the object serializers.
    /// </summary>
    /// <seealso cref="Apworks.IObjectSerializer" />
    public abstract class ObjectSerializer : IObjectSerializer
    {
        /// <summary>
        /// Deserializes the object from a <see cref="byte" /> array.
        /// </summary>
        /// <typeparam name="TObject">The type of the object to be deserialized.</typeparam>
        /// <param name="data">The <see cref="byte" /> array which contains the object data.</param>
        /// <returns>
        /// The deserialized object
        /// </returns>
        public virtual TObject Deserialize<TObject>(byte[] data) => (TObject)Deserialize(data, typeof(TObject));

        /// <summary>
        /// Deserializes the object from a <see cref="byte" /> array.
        /// </summary>
        /// <param name="data">The <see cref="byte" /> array which contains the object data.</param>
        /// <param name="objType">The type of the object to be deserialized.</param>
        /// <returns>
        /// The deserialized object.
        /// </returns>
        public abstract object Deserialize(byte[] data, Type objType);

        /// <summary>
        /// Deserializes the object from a <see cref="byte" /> array asynchronously.
        /// </summary>
        /// <typeparam name="TObject">The type of the object to be deserialized.</typeparam>
        /// <param name="data">The <see cref="byte" /> array which contains the object data.</param>
        /// <param name="cancellationToken">The cancellation token which propagates notification that operations should be canceled.</param>
        /// <returns>
        /// The deserialized object.
        /// </returns>
        public virtual Task<TObject> DeserializeAsync<TObject>(byte[] data, CancellationToken cancellationToken = default(CancellationToken)) => Task.FromResult(Deserialize<TObject>(data));

        /// <summary>
        /// the object from a <see cref="byte" /> array asynchronously.
        /// </summary>
        /// <param name="data">The <see cref="byte" /> array which contains the object data.</param>
        /// <param name="objType">The type of the object to be deserialized.</param>
        /// <param name="cancellationToken">The cancellation token which propagates notification that operations should be canceled.</param>
        /// <returns>
        /// The deserialized object.
        /// </returns>
        public virtual Task<object> DeserializeAsync(byte[] data, Type objType, CancellationToken cancellationToken = default(CancellationToken)) => Task.FromResult(Deserialize(data, objType));

        /// <summary>
        /// Serializes the specified object into a <see cref="byte" /> array.
        /// </summary>
        /// <typeparam name="TObject">The type of the object to be serialized.</typeparam>
        /// <param name="obj">The object to be serialized.</param>
        /// <returns>
        /// The byte array which contains the serialized data.
        /// </returns>
        public virtual byte[] Serialize<TObject>(TObject obj) => Serialize(typeof(TObject), obj);

        /// <summary>
        /// Serializes the specified object into a <see cref="byte" /> array.
        /// </summary>
        /// <param name="objType">The type of the object to be serialized.</param>
        /// <param name="obj">The object to be serialized.</param>
        /// <returns>
        /// The byte array which contains the serialized data.
        /// </returns>
        public abstract byte[] Serialize(Type objType, object obj);

        /// <summary>
        /// Serializes the specified object into a <see cref="byte" /> array asynchronously.
        /// </summary>
        /// <typeparam name="TObject">The type of the object to be serialized.</typeparam>
        /// <param name="obj">The object to be serialized.</param>
        /// <param name="cancellationToken">The cancellation token which propagates notification that operations should be canceled.</param>
        /// <returns>
        /// The byte array which contains the serialized data.
        /// </returns>
        public virtual Task<byte[]> SerializeAsync<TObject>(TObject obj, CancellationToken cancellationToken = default(CancellationToken)) => Task.FromResult(Serialize(obj));

        /// <summary>
        /// Serializes the specified object into a <see cref="byte" /> array
        /// </summary>
        /// <param name="objType">The type of the object to be serialized.</param>
        /// <param name="obj">The object to be serialized.</param>
        /// <param name="cancellationToken">The cancellation token which propagates notification that operations should be canceled.</param>
        /// <returns>
        /// The byte array which contains the serialized data.
        /// </returns>
        public virtual Task<byte[]> SerializeAsync(Type objType, object obj, CancellationToken cancellationToken = default(CancellationToken)) => Task.FromResult(Serialize(objType, obj));
    }
}
