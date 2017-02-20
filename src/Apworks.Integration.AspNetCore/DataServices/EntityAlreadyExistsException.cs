using System;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Apworks.Integration.AspNetCore.DataServices
{
    public sealed class EntityAlreadyExistsException : DataServiceException
    {
        public EntityAlreadyExistsException(string message)
            : base(HttpStatusCode.Conflict, message)
        { }

        public EntityAlreadyExistsException(string format, params object[] args)
            : base(HttpStatusCode.Conflict, format, args)
        { }
    }
}
