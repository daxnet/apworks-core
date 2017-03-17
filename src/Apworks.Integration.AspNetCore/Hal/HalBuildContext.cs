using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
