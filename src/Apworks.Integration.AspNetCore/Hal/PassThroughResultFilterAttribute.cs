using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Apworks.Integration.AspNetCore.Hal
{
    /// <summary>
    /// Represents the result filter that does nothing on the action result.
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.Filters.ResultFilterAttribute" />
    internal sealed class PassThroughResultFilterAttribute : ResultFilterAttribute
    {
    }
}
