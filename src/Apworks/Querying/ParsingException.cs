using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apworks.Querying
{
    public class ParsingException : QueryingException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ParsingException"/> class.
        /// </summary>
        public ParsingException(IEnumerable<string> parseErrors)
            : base()
        {
            this.ParseErrors = parseErrors;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ParsingException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public ParsingException(string message, IEnumerable<string> parseErrors)
            : base(message)
        {
            this.ParseErrors = parseErrors;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ParsingException"/> class.
        /// </summary>
        /// <param name="format">The format of the error message.</param>
        /// <param name="args">The arguments to be used for constructing the error message.</param>
        public ParsingException(string format, IEnumerable<string> parseErrors, params object[] args)
            : base(string.Format(format, args))
        {
            this.ParseErrors = parseErrors;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ParsingException"/> class.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified.</param>
        public ParsingException(string message, IEnumerable<string> parseErrors, Exception innerException)
            : base(message, innerException)
        {
            this.ParseErrors = parseErrors;
        }

        public IEnumerable<string> ParseErrors { get; }
    }
}
