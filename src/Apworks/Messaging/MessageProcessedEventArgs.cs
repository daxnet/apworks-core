using System;

namespace Apworks.Messaging
{
    /// <summary>
    /// Represents the event data that is generated when the message has been processed.
    /// </summary>
    public class MessageProcessedEventArgs : EventArgs
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageProcessedEventArgs"/> class.
        /// </summary>
        /// <param name="message">The message that has been processed.</param>
        public MessageProcessedEventArgs(IMessage message)
        {
            this.Message = message;
        }

        /// <summary>
        /// Gets the message that has been processed.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        public IMessage Message { get; }
    }
}
