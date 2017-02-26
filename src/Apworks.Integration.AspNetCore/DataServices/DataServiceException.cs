using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Apworks.Integration.AspNetCore.DataServices
{
    public abstract class DataServiceException : ApworksException
    {
        private readonly HttpStatusCode httpStatusCode;

        protected DataServiceException(HttpStatusCode httpStatusCode)
        {
            this.httpStatusCode = httpStatusCode;
        }

        protected DataServiceException(HttpStatusCode httpStatusCode, string message)
            : base(message)
        {
            this.httpStatusCode = httpStatusCode;
        }

        protected DataServiceException(HttpStatusCode httpStatusCode, string format, params object[] args)
            : base(format, args)
        {
            this.httpStatusCode = httpStatusCode;
        }

        protected DataServiceException(HttpStatusCode httpStatusCode, string message, Exception innerException)
            : base(message, innerException)
        {
            this.httpStatusCode = httpStatusCode;
        }

        public HttpStatusCode StatusCode => httpStatusCode;
    }
}
