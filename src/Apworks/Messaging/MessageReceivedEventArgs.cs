using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apworks.Messaging
{
    /// <summary>
    /// Represents the event data that is generated when the message has been received.
    /// </summary>
    /// <seealso cref="Apworks.Messaging.MessageProcessedEventArgs" />
    public sealed class MessageReceivedEventArgs : MessageProcessedEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MessageReceivedEventArgs"/> class.
        /// </summary>
        /// <param name="message">The message that has been processed.</param>
        public MessageReceivedEventArgs(IMessage message) : base(message)
        {
        }
    }
}
