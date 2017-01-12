using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Apworks.Integration.AspNetCore.Hal
{
    /// <summary>
    /// Represents the option of HAL generation.
    /// </summary>
    public sealed class HalGenerationOption
    {
        /// <summary>
        /// Represents the default generation option, that is, ignoring the Null values and generate the formatted HAL.
        /// </summary>
        public static readonly HalGenerationOption Default = new HalGenerationOption();

        /// <summary>
        /// Represents the generation option that is ignoring the Null values and generating the non-formatted HAL.
        /// </summary>
        public static readonly HalGenerationOption NoFormat = new HalGenerationOption(HalFormat.None);

        /// <summary>
        /// Initializes a new instance of the <see cref="HalGenerationOption"/> class.
        /// </summary>
        /// <param name="format">The <see cref="HalFormat"/> value which indicates the formation of the HAL.</param>
        /// <param name="ignoreNullValues"><c>True</c> if the Null values should be ignored in the generated HAL, otherwise, <c>False</c>.</param>
        public HalGenerationOption(HalFormat format = HalFormat.Formatted, bool ignoreNullValues = true)
        {
            this.Format = format;
            this.IgnoreNullValues = ignoreNullValues;
        }

        /// <summary>
        /// Gets the <see cref="HalFormat"/> value which indicates the formation of the HAL.
        /// </summary>
        /// <value>
        /// The <see cref="HalFormat"/> value which indicates the formation of the HAL.
        /// </value>
        public HalFormat Format { get; }

        /// <summary>
        /// Gets a value indicating whether the Null values should be ignored in the generated HAL.
        /// </summary>
        /// <value>
        ///   <c>True</c> if the Null values should be ignored in the generated HAL, otherwise, <c>False</c>.
        /// </value>
        public bool IgnoreNullValues { get; }
    }
}
