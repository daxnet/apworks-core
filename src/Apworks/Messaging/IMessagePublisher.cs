using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Apworks.Messaging
{
    /// <summary>
    /// Represents that the implemented classes are message publishers that can
    /// publish the specified message to a message bus.
    /// </summary>
    public interface IMessagePublisher : IDisposable
    {
        /// <summary>
        /// Publishes the specified message.
        /// </summary>
        /// <typeparam name="TMessage">The type of the message to be published.</typeparam>
        /// <param name="message">The message that is going to be published.</param>
        void Publish<TMessage>(TMessage message)
            where TMessage : IMessage;

        void PublishAll(IEnumerable<IMessage> messages);

        /// <summary>
        /// publishes the specified message asynchronously.
        /// </summary>
        /// <typeparam name="TMessage">The type of the message to be published.</typeparam>
        /// <param name="message">The message that is going to be published.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> instance that propagates notification that operations should be canceled.</param>
        /// <returns></returns>
        Task PublishAsync<TMessage>(TMessage message, CancellationToken cancellationToken = default(CancellationToken))
            where TMessage : IMessage;

        /// <summary>
        /// Publishes a series of messages with the same message type asynchronously.
        /// </summary>
        /// <typeparam name="TMessage">The type of the messages to be published.</typeparam>
        /// <param name="messages">A series of messages to be published.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> which propagates notification that operations should be canceled.</param>
        /// <returns></returns>
        Task PublishAllAsync(IEnumerable<IMessage> messages, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Represents the event that occurs when the message has been published.
        /// </summary>
        event EventHandler<MessagePublishedEventArgs> MessagePublished;
    }
}
