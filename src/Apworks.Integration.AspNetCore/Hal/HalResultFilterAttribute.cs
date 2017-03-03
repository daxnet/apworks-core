using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apworks.Integration.AspNetCore.Hal
{
    internal sealed class HalResultFilterAttribute : ResultFilterAttribute
    {
        private const string HalJsonContentType = "application/hal+json";
        private readonly IHalBuildConfiguration configuration;

        public HalResultFilterAttribute(IHalBuildConfiguration configuration)
        {
            this.configuration = configuration;
        }

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

                    var state = objectResult.Value;
                    var halBuilder = halBuilderFactory(new HalBuildContext(state, actionDescriptor));
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
