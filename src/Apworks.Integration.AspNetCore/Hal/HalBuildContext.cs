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

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;

namespace Apworks.Integration.AspNetCore.Hal
{
    /// <summary>
    /// Represents the context that contains the request/response data for generating the HAL builder.
    /// </summary>
    public sealed class HalBuildContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HalBuildContext"/> class.
        /// </summary>
        /// <param name="state">The state which contains the action execution result.</param>
        /// <param name="httpContext">The HTTP context.</param>
        /// <param name="urlHelper">The URL helper for building the URLs in the ASP.NET MVC application.</param>
        /// <param name="controllerAction">The controller action descriptor instance which contains the information about controllers and actions.</param>
        public HalBuildContext(object state, HttpContext httpContext, IUrlHelper urlHelper, ControllerActionDescriptor controllerAction)
        {
            this.State = state;
            this.HttpContext = httpContext;
            this.UrlHelper = urlHelper;
            this.ControllerAction = controllerAction;
        }

        /// <summary>
        /// Gets the state which contains the action execution result.
        /// </summary>
        public object State { get; }

        public HttpContext HttpContext { get; }

        public IUrlHelper UrlHelper { get; }

        public ControllerActionDescriptor ControllerAction { get; }
    }
}
