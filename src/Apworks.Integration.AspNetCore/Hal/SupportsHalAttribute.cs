using Microsoft.AspNetCore.Mvc.Filters;
using System;

namespace Apworks.Integration.AspNetCore.Hal
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public sealed class SupportsHalAttribute : Attribute, IFilterFactory
    {
        /// <summary>
        /// Gets a value that indicates if the result of <see cref="M:Microsoft.AspNetCore.Mvc.Filters.IFilterFactory.CreateInstance(System.IServiceProvider)" />
        /// can be reused across requests.
        /// </summary>
        public bool IsReusable => true;

        /// <summary>
        /// Creates an instance of the executable filter.
        /// </summary>
        /// <param name="serviceProvider">The request <see cref="T:System.IServiceProvider" />.</param>
        /// <returns>
        /// An instance of the executable filter.
        /// </returns>
        public IFilterMetadata CreateInstance(IServiceProvider serviceProvider)
        {
            var halBuildConfiguration = (IHalBuildConfiguration)serviceProvider.GetService(typeof(IHalBuildConfiguration));

            if (halBuildConfiguration == null)
            {
                return new PassThroughResultFilterAttribute();
            }

            return new HalResultFilterAttribute(halBuildConfiguration);
        }
    }
}
