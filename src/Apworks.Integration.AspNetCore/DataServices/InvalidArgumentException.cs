using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Apworks.Integration.AspNetCore.DataServices
{
    public sealed class InvalidArgumentException : DataServiceException
    {
        public InvalidArgumentException(string message)
            : base(HttpStatusCode.BadRequest, message)
        { }

        public InvalidArgumentException(string format, params object[] args)
            : base(HttpStatusCode.BadRequest, format, args)
        { }
    }
}
