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

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Routing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apworks.Integration.AspNetCore.Hal
{
    /// <summary>
    /// Represents the result filter that will apply the HAL content based on the original action execution result.
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.Filters.ResultFilterAttribute" />
    internal sealed class HalResultFilterAttribute : ResultFilterAttribute
    {
        private const string HalJsonContentType = "application/hal+json";
        private readonly IHalBuildConfiguration configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="HalResultFilterAttribute"/> class.
        /// </summary>
        /// <param name="configuration">The HAL build configuration.</param>
        public HalResultFilterAttribute(IHalBuildConfiguration configuration)
        {
            this.configuration = configuration;
        }

        /// <summary>
        /// </summary>
        /// <param name="context"></param>
        /// <param name="next"></param>
        /// <returns></returns>
        /// <inheritdoc />
        public override async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {
            var actionDescriptor = context.ActionDescriptor as ControllerActionDescriptor;

            if (actionDescriptor != null)
            {
                var objectResult = context.Result as ObjectResult;
                var halBuilderFactory = this.configuration.RetrieveHalBuilderFactory(new ControllerActionSignature(actionDescriptor.ControllerName, 
                    actionDescriptor.ActionName,
                    actionDescriptor.MethodInfo.GetParameters().Select(x => x.ParameterType)));

                if (halBuilderFactory != null && objectResult != null)
                {
                    var urlHelper = new UrlHelper(context);
                    var state = objectResult.Value;
                    var halBuilder = halBuilderFactory(new HalBuildContext(state, context.HttpContext, urlHelper, actionDescriptor));
                    var resource = halBuilder.Build();
                    var json = resource.ToString();

                    var bytes = Encoding.UTF8.GetBytes(json);
                    context.HttpContext.Response.ContentLength = bytes.Length;
                    context.HttpContext.Response.ContentType = HalJsonContentType;
                    using (var ms = new MemoryStream(bytes))
                    {
                        await ms.CopyToAsync(context.HttpContext.Response.Body);
                    }

                    return;
                }
            }

            await base.OnResultExecutionAsync(context, next);
        }
    }
}
