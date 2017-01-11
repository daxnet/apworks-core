using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Apworks.Integration.AspNetCore.Hal
{
    /// <summary>
    /// Represents the format of the generated HAL.
    /// </summary>
    public enum HalFormat
    {
        /// <summary>
        /// Indicates that the generation of the HAL should not be formatted.
        /// </summary>
        None,

        /// <summary>
        /// Indicates that the generation of the HAL should be formatted.
        /// </summary>
        Formatted
    }
}
