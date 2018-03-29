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

using Microsoft.AspNetCore.Http;
using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Apworks.Integration.AspNetCore.DataServices
{
    /// <summary>
    /// Represents the middleware that handles <see cref="DataServiceException"/>
    /// in the ASP.NET invocation chain.
    /// </summary>
    public sealed class DataServiceExceptionHandler
    {
        #region Fields
        private readonly RequestDelegate nextInvocation;
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="DataServiceExceptionHandler"/> class.
        /// </summary>
        /// <param name="nextInvocation">The function that can process an HTTP request.</param>
        public DataServiceExceptionHandler(RequestDelegate nextInvocation)
        {
            this.nextInvocation = nextInvocation;
        }

        /// <summary>
        /// Invokes the function that processes an HTTP request on the given HTTP context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>The <see cref="Task"/> object that processes the HTTP request.</returns>
        public async Task Invoke(HttpContext context)
        {
            try
            {
                await this.nextInvocation.Invoke(context);
            }
            catch(ArgumentException ex)
            {
                await ConstructExceptionResponseAsync(context, new InvalidArgumentException(ex.Message, ex));
            }
            catch(Exception ex)
            {
                await ConstructExceptionResponseAsync(context, ex);
            }
        }

        private static async Task ConstructExceptionResponseAsync(HttpContext context, Exception exception)
        {
            var statusCode = HttpStatusCode.InternalServerError;
            var dataServiceException = exception as DataServiceException;
            if (dataServiceException != null)
            {
                statusCode = dataServiceException.StatusCode;
            }

            var message = exception.ToString();
            context.Response.StatusCode = Convert.ToInt32(statusCode);
            context.Response.ContentLength = Encoding.UTF8.GetBytes(message).Length;
            context.Response.ContentType = "text/plain; charset=utf-8";
            await context.Response.WriteAsync(message);
        }
    }
}
