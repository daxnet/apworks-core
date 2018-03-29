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

using Apworks.Querying;
using Hal.Builders;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.Primitives;
using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;

namespace Apworks.Integration.AspNetCore.Hal
{
    /// <summary>
    /// Represents the HAL build configuration that builds the HAL notation based on the <see cref="IPagedResult"/> instance.
    /// </summary>
    /// <seealso cref="Apworks.Integration.AspNetCore.Hal.HalBuildConfiguration" />
    public class PagedResultHalBuildConfiguration : HalBuildConfiguration
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PagedResultHalBuildConfiguration"/> class.
        /// </summary>
        /// <param name="signatures">The controller action signatures which describes the controller actions
        /// whose return value will be represented in HAL notation.</param>
        public PagedResultHalBuildConfiguration(IEnumerable<ControllerActionSignature> signatures)
        {
            foreach (var signature in signatures)
            {
                this.RegisterHalBuilderFactory(signature, this.GetPagedResultHalBuildFactory);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PagedResultHalBuildConfiguration"/> class.
        /// </summary>
        /// <param name="signatures">The controller action signatures which describes the controller actions
        /// whose return value will be represented in HAL notation.</param>
        public PagedResultHalBuildConfiguration(params string[] signatures)
        {
            foreach (var signature in signatures)
            {
                this.RegisterHalBuilderFactory(signature, this.GetPagedResultHalBuildFactory);
            }
        }

        /// <summary>
        /// The helper method which returns the HAL build factory that can build the HAL with the <see cref="IPagedResult"/>
        /// object.
        /// </summary>
        /// <param name="context">The <see cref="HalBuildContext"/> object that carries the information for creating
        /// the HAL builder.</param>
        /// <returns>The <see cref="IBuilder"/> object which can build a HAL data structure.</returns>
        protected IBuilder GetPagedResultHalBuildFactory(HalBuildContext context)
        {
            if (!(context.State is IPagedResult))
            {
                throw new HalBuildConfigurationException("The state returned from the controller action cannot be converted into IPagedResult instance.");
            }

            var state = (IPagedResult)context.State;
            var pageSize = state.PageSize;
            var pageNumber = state.PageNumber <= 0 ? 1 : state.PageNumber;
            var totalRecords = state.TotalRecords;
            var totalPages = state.TotalPages;
            var selfLinkItem = context.HttpContext.Request.GetEncodedUrl();
            var pageNumberParameterName = InferPageNumberParameterName(context.ControllerAction);

            var linkItemBuilder = new ResourceBuilder()
               .WithState(new { pageNumber, pageSize, totalRecords, totalPages })
               .AddSelfLink().WithLinkItem(selfLinkItem);

            if (!string.IsNullOrEmpty(pageNumberParameterName))
            {
                var firstLinkItem = GenerateLink(context.HttpContext.Request, new Dictionary<string, StringValues> { { pageNumberParameterName, 1.ToString() } });
                linkItemBuilder = linkItemBuilder.AddLink("first").WithLinkItem(firstLinkItem);

                var lastLinkItem = GenerateLink(context.HttpContext.Request, new Dictionary<string, StringValues> { { pageNumberParameterName, totalPages.ToString() } });
                linkItemBuilder = linkItemBuilder.AddLink("last").WithLinkItem(lastLinkItem);

                string prevLinkItem = null;
                if (pageNumber > 1 && pageNumber <= totalPages)
                {
                    prevLinkItem = GenerateLink(context.HttpContext.Request, new Dictionary<string, StringValues> { { pageNumberParameterName, (pageNumber - 1).ToString() } });
                    linkItemBuilder = linkItemBuilder.AddLink("prev").WithLinkItem(prevLinkItem);
                }

                string nextLinkItem = null;
                if (pageNumber >= 1 && pageNumber < totalPages)
                {
                    nextLinkItem = GenerateLink(context.HttpContext.Request, new Dictionary<string, StringValues> { { pageNumberParameterName, (pageNumber + 1).ToString() } });
                    linkItemBuilder = linkItemBuilder.AddLink("next").WithLinkItem(nextLinkItem);
                }
            }

            var embeddedResourceName = context.ControllerAction.ControllerName.ToLower();
            var attribute = context.ControllerAction.MethodInfo.CustomAttributes.FirstOrDefault(c => c.AttributeType == typeof(HalEmbeddedResourceAttribute));
            if (attribute != null)
            {
                embeddedResourceName = (string)attribute.ConstructorArguments.FirstOrDefault(x => x.ArgumentType == typeof(string)).Value;
            }

            var resourceBuilder = linkItemBuilder.AddEmbedded(embeddedResourceName)
                .Resource(new ResourceBuilder().WithState(context.State));

            return resourceBuilder;
        }

        #region Private Methods
        /// <summary>
        /// Generates the hyperlink to a resource with the query values being substituted.
        /// </summary>
        /// <param name="request">The <see cref="HttpRequest"/> instance which contains the HTTP request information.</param>
        /// <param name="querySubstitution">The key-value pair collection which contains the values of the keys that will substitute the original values in the HTTP request query object.</param>
        /// <returns>A hyperlink with the query values being substituted.</returns>
        private static string GenerateLink(HttpRequest request, IEnumerable<KeyValuePair<string, StringValues>> querySubstitution)
        {
            var scheme = request.Scheme;
            var host = request.Host;
            var pathBase = request.PathBase;
            var path = request.Path;
            var substQuery = new Dictionary<string, StringValues>();

            if (request.Query?.Count > 0)
            {
                request.Query.ToList().ForEach(q => substQuery.Add(q.Key, q.Value));
                foreach(var subst in querySubstitution)
                {
                    if (substQuery.ContainsKey(subst.Key))
                    {
                        substQuery[subst.Key] = subst.Value;
                    }
                    else
                    {
                        substQuery.Add(subst.Key, subst.Value);
                    }
                }
            }
            else
            {
                querySubstitution.ToList().ForEach(kvp => substQuery.Add(kvp.Key, kvp.Value));
            }

            return UriHelper.BuildAbsolute(scheme, host, pathBase, path, QueryString.Create(substQuery), default(FragmentString));
        }

        /// <summary>
        /// Infers the name of the page number parameter.
        /// </summary>
        /// <param name="cad">The <see cref="ControllerActionDescriptor"/> which contains the controller action definition information.</param>
        /// <returns>The name of the page number parameter, or null, if the name cannot be inferred.</returns>
        private static string InferPageNumberParameterName(ControllerActionDescriptor cad)
        {
            if (cad.Parameters.Any(p => p.ParameterType == typeof(int) && p.Name.Equals("page", StringComparison.OrdinalIgnoreCase)))
            {
                return "page";
            }

            if (cad.Parameters.Any(p => p.ParameterType == typeof(int) && p.Name.Equals("pageNumber", StringComparison.OrdinalIgnoreCase)))
            {
                return "pageNumber";
            }

            var pageNumberParameter = cad.MethodInfo.GetParameters().FirstOrDefault(x => x.IsDefined(typeof(PageNumberAttribute)));
            if (pageNumberParameter != null)
            {
                return pageNumberParameter.Name;
            }

            return null;
        }
        #endregion
    }
}
