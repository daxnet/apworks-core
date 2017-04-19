using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Apworks.Messaging
{
    /// <summary>
    /// Represents that the implemented classes are message handlers.
    /// </summary>
    /// <typeparam name="TMessage">The type of the message to be handled by current handler.</typeparam>
    public interface IMessageHandler<in TMessage>
        where TMessage : IMessage
    {
        /// <summary>
        /// Handles the specified message.
        /// </summary>
        /// <param name="message">The message to be handled.</param>
        /// <returns><c>true</c> if the message has been handled properly, otherwise, <c>false</c>.</returns>
        bool Handle(TMessage message);

        /// <summary>
        /// Handles the specified message asynchronously.
        /// </summary>
        /// <param name="message">The message to be handled.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> instance which propagates notification that operations should be canceled.</param>
        /// <returns><c>true</c> if the message has been handled properly, otherwise, <c>false</c>.</returns>
        Task<bool> HandleAsync(TMessage message, CancellationToken cancellationToken = default(CancellationToken));
    }
}
