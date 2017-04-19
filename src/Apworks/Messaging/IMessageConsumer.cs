using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apworks.Messaging
{
    /// <summary>
    /// Represents that the implemented classes are message consumers that will use
    /// its internal message subscriber to subscribe the message bus and take specific
    /// actions when there is any incoming messages.
    /// </summary>
    /// <seealso cref="System.IDisposable" />
    public interface IMessageConsumer : IDisposable
    {
        /// <summary>
        /// Gets the instance of <see cref="IMessageSubscriber"/> which subscribes
        /// to the message bus and notifies events when there is any incoming messages.
        /// </summary>
        IMessageSubscriber Subscriber { get; }
    }
}
