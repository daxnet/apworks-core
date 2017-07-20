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

using Apworks.Integration.AspNetCore.Hal;
using Apworks.Querying;
using Hal.Builders;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Apworks.Integration.AspNetCore.DataServices
{
    /// <summary>
    /// Represents the HAL build configuration that configures the HAL builder
    /// factory for data services.
    /// </summary>
    public class DataServiceHalBuildConfiguration : HalBuildConfiguration
    {
        #region Ctor
        /// <summary>
        /// Initializes a new instance of the <see cref="DataServiceHalBuildConfiguration"/> class.
        /// </summary>
        public DataServiceHalBuildConfiguration()
        {
            this.RegisterHalBuilderFactoryForGet();
            this.RegisterHalBuilderFactoryForGetAll();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataServiceHalBuildConfiguration"/> class.
        /// </summary>
        /// <param name="halBuilderFactories">The hal builder factories.</param>
        public DataServiceHalBuildConfiguration(IEnumerable<KeyValuePair<ControllerActionSignature, Func<HalBuildContext, IBuilder>>> halBuilderFactories)
            : this()
        {
            foreach(var factory in halBuilderFactories)
            {
                this.RegisterHalBuilderFactory(factory.Key, factory.Value);
            }
        }
        #endregion

        #region Protected Methods
        /// <summary>
        /// Registers the HAL builder factory for the default HTTP GET method, which returns a collection
        /// of the aggregates with pagination enabled and a search criteria applied.
        /// </summary>
        protected virtual void RegisterHalBuilderFactoryForGetAll()
        {
            this.RegisterHalBuilderFactory("*.Get(int, int, *, *)", this.GetPagedResultHalBuildFactory);
        }

        /// <summary>
        /// Registers the HAL builder factory for the HTTP GET method that returns a particular aggregate
        /// with a given aggregate root key.
        /// </summary>
        protected virtual void RegisterHalBuilderFactoryForGet()
        {
            this.RegisterHalBuilderFactory("*.Get(*)", context =>
            {
                return new ResourceBuilder()
                    .WithState(null)
                    .AddSelfLink().WithLinkItem(context.HttpContext.Request.GetDisplayUrl())
                    .AddEmbedded(context.State.GetType().Name.ToLower())
                    .Resource(new ResourceBuilder().WithState(context.State));
            });
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
            var state = (IPagedResult)context.State;
            var pageSize = state.PageSize;
            var pageNumber = state.PageNumber;
            var totalRecords = state.TotalRecords;
            var totalPages = state.TotalPages;
            var selfLinkItem = context.HttpContext.Request.GetEncodedUrl();

            string prevLinkItem = null;
            if (pageNumber > 1 && pageNumber <= totalPages)
            {
                prevLinkItem = GenerateLink(context.HttpContext.Request, new Dictionary<string, StringValues> { { "page", (pageNumber - 1).ToString() } });
            }

            string nextLinkItem = null;
            if (pageNumber < totalPages)
            {
                nextLinkItem = GenerateLink(context.HttpContext.Request, new Dictionary<string, StringValues> { { "page", (pageNumber + 1).ToString() } });
            }

            var linkItemBuilder = new ResourceBuilder()
                .WithState(new { pageNumber, pageSize, totalRecords, totalPages })
                .AddSelfLink().WithLinkItem(selfLinkItem);

            if (!string.IsNullOrEmpty(prevLinkItem))
            {
                linkItemBuilder = linkItemBuilder.AddLink("prev").WithLinkItem(prevLinkItem);
            }

            if (!string.IsNullOrEmpty(nextLinkItem))
            {
                linkItemBuilder = linkItemBuilder.AddLink("next").WithLinkItem(nextLinkItem);
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
        #endregion

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
                foreach (var queryItem in request.Query)
                {
                    if (querySubstitution.Any(item => item.Key == queryItem.Key))
                    {
                        substQuery[queryItem.Key] = querySubstitution.First(item => item.Key == queryItem.Key).Value;
                    }
                    else
                    {
                        substQuery[queryItem.Key] = queryItem.Value;
                    }
                }
            }
            
            return UriHelper.BuildAbsolute(scheme, host, pathBase, path, QueryString.Create(substQuery), default(FragmentString));
        }
        #endregion
    }
}
