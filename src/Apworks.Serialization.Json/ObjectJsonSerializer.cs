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

using Newtonsoft.Json;
using System;
using System.Text;

namespace Apworks.Serialization.Json
{
    /// <summary>
    /// Represents the object serializer that serializes an object into 
    /// </summary>
    /// <seealso cref="Apworks.ObjectSerializer" />
    public sealed class ObjectJsonSerializer : ObjectSerializer
    {
        private readonly Encoding encoding;

        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectJsonSerializer"/> class.
        /// </summary>
        /// <param name="encoding">The encoding to be used for serialize/deserialize the object.</param>
        public ObjectJsonSerializer(Encoding encoding = null)
            => this.encoding = encoding ?? Encoding.UTF8;

        /// <summary>
        /// Serializes the specified object into a <see cref="T:System.Byte" /> array.
        /// </summary>
        /// <param name="objType">The type of the object to be serialized.</param>
        /// <param name="obj">The object to be serialized.</param>
        /// <returns>
        /// The byte array which contains the serialized data.
        /// </returns>
        public override byte[] Serialize(Type objType, object obj)
            => this.encoding.GetBytes(JsonConvert.SerializeObject(obj, objType, Formatting.Indented, null));

        /// <summary>
        /// Deserializes the object from a <see cref="T:System.Byte" /> array.
        /// </summary>
        /// <param name="data">The <see cref="T:System.Byte" /> array which contains the object data.</param>
        /// <param name="objType">The type of the object to be deserialized.</param>
        /// <returns>
        /// The deserialized object.
        /// </returns>
        public override object Deserialize(byte[] data, Type objType)
            => JsonConvert.DeserializeObject(this.encoding.GetString(data), objType);

        /// <summary>
        /// Deserializes the object from a <see cref="T:System.Byte" /> array.
        /// </summary>
        /// <typeparam name="TObject">The type of the object to be deserialized.</typeparam>
        /// <param name="data">The <see cref="T:System.Byte" /> array which contains the object data.</param>
        /// <returns>
        /// The deserialized object
        /// </returns>
        public override TObject Deserialize<TObject>(byte[] data)
            => JsonConvert.DeserializeObject<TObject>(this.encoding.GetString(data));
    }
}
