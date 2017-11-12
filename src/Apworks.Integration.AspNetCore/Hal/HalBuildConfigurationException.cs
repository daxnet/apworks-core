using System;
using System.Collections.Generic;
using System.Text;

namespace Apworks.Integration.AspNetCore.Hal
{
    public class HalBuildConfigurationException : ApworksException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HalBuildConfigurationException"/> class.
        /// </summary>
        public HalBuildConfigurationException()
            : base()
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="HalBuildConfigurationException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public HalBuildConfigurationException(string message)
            : base(message)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="HalBuildConfigurationException"/> class.
        /// </summary>
        /// <param name="format">The format of the error message.</param>
        /// <param name="args">The arguments to be used for constructing the error message.</param>
        public HalBuildConfigurationException(string format, params object[] args)
            : base(string.Format(format, args))
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="HalBuildConfigurationException"/> class.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified.</param>
        public HalBuildConfigurationException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
