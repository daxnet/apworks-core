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
using System.Net;

namespace Apworks.Integration.AspNetCore.DataServices
{
    /// <summary>
    /// Represents the error that occurs in the Apworks Data Services implementation.
    /// </summary>
    /// <seealso cref="Apworks.ApworksException" />
    public abstract class DataServiceException : ApworksException
    {
        #region Fields
        private readonly HttpStatusCode httpStatusCode;
        #endregion

        #region Ctor
        /// <summary>
        /// Initializes a new instance of the <see cref="DataServiceException"/> class.
        /// </summary>
        /// <param name="httpStatusCode">The HTTP status code that represents the error.</param>
        protected DataServiceException(HttpStatusCode httpStatusCode)
        {
            this.httpStatusCode = httpStatusCode;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataServiceException"/> class.
        /// </summary>
        /// <param name="httpStatusCode">The HTTP status code that represents the error.</param>
        /// <param name="message">The message that describes the error.</param>
        protected DataServiceException(HttpStatusCode httpStatusCode, string message)
            : base(message)
        {
            this.httpStatusCode = httpStatusCode;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataServiceException"/> class.
        /// </summary>
        /// <param name="httpStatusCode">The HTTP status code that represents the error.</param>
        /// <param name="format">The string format pattern that formats a string with the given arguments.</param>
        /// <param name="args">The arguments used for formatting the message string.</param>
        protected DataServiceException(HttpStatusCode httpStatusCode, string format, params object[] args)
            : base(format, args)
        {
            this.httpStatusCode = httpStatusCode;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataServiceException"/> class.
        /// </summary>
        /// <param name="httpStatusCode">The HTTP status code that represents the error.</param>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="innerException">The inner exception.</param>
        protected DataServiceException(HttpStatusCode httpStatusCode, string message, Exception innerException)
            : base(message, innerException)
        {
            this.httpStatusCode = httpStatusCode;
        }
        #endregion

        #region Properties        
        /// <summary>
        /// Gets the HTTP status code that represents the error.
        /// </summary>
        /// <value>
        /// The status code.
        /// </value>
        public HttpStatusCode StatusCode => httpStatusCode;
        #endregion
    }
}
