using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apworks.Messaging
{
    public sealed class MessageProcessingException : ApworksException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MessageProcessingException"/> class.
        /// </summary>
        public MessageProcessingException()
            : base()
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageProcessingException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public MessageProcessingException(string message)
            : base(message)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageProcessingException"/> class.
        /// </summary>
        /// <param name="format">The format of the error message.</param>
        /// <param name="args">The arguments to be used for constructing the error message.</param>
        public MessageProcessingException(string format, params object[] args)
            : base(string.Format(format, args))
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageProcessingException"/> class.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified.</param>
        public MessageProcessingException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
