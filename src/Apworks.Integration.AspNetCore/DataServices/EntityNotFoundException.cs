using System;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Apworks.Integration.AspNetCore.DataServices
{
    public sealed class EntityNotFoundException : DataServiceException
    {
        public EntityNotFoundException(string message)
            : base(HttpStatusCode.NotFound, message)
        { }

        public EntityNotFoundException(string format, params object[] args)
            : base(HttpStatusCode.NotFound, format, args)
        { }
    }
}
