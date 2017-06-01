using Apworks.Integration.AspNetCore.Hal;
using Apworks.Querying;
using Hal.Builders;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Primitives;
using System.Collections.Generic;
using System.Linq;

namespace Apworks.Integration.AspNetCore.DataServices
{
    /// <summary>
    /// Represents the HAL build configuration which configures the HAL builder
    /// factory for data services.
    /// </summary>
    public class DataServiceHalBuildConfiguration : HalBuildConfiguration
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataServiceHalBuildConfiguration"/> class.
        /// </summary>
        public DataServiceHalBuildConfiguration()
        {
            this.RegisterHalBuilderFactoryForGet();
            this.RegisterHalBuilderFactoryForGetAll();
        }

        /// <summary>
        /// Registers the hal builder factory for the default HTTP GET method, which returns a collection
        /// of the aggregates with pagination enabled and a search criteria applied.
        /// </summary>
        protected virtual void RegisterHalBuilderFactoryForGetAll()
        {
            this.RegisterHalBuilderFactory("*.Get(int, int, *, *)", this.GetPagedResultHalBuildFactory);
        }

        /// <summary>
        /// Registers the hal builder factory for the HTTP GET method that returns a particular aggregate
        /// with a given aggregate root Id.
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
    }
}
