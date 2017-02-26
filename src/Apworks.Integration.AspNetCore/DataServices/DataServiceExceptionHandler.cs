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
        private readonly RequestDelegate nextInvocation;

        public DataServiceExceptionHandler(RequestDelegate nextInvocation)
        {
            this.nextInvocation = nextInvocation;
        }

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
