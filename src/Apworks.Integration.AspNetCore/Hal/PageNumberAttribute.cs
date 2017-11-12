using System;
using System.Collections.Generic;
using System.Text;

namespace Apworks.Integration.AspNetCore.Hal
{
    /// <summary>
    /// Represents that the decorated parameter represents the page number in a pagination operation.
    /// </summary>
    /// <seealso cref="System.Attribute" />
    [AttributeUsage(AttributeTargets.Parameter)]
    public sealed class PageNumberAttribute : Attribute
    {
    }
}
