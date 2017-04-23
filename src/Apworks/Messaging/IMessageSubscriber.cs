using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apworks.Messaging
{
    /// <summary>
    /// Represents that the implemented classes are message subscribers that listen to
    /// the underlying messaging infrastructure and notify the observers when there is
    /// any incoming messages.
    /// </summary>
    /// <seealso cref="System.IDisposable" />
    public interface IMessageSubscriber : IDisposable
    {
        /// <summary>
        /// Subscribes to the underlying messaging infrastructure.
        /// </summary>
        void Subscribe();

        /// <summary>
        /// Occurs when there is any incoming messages.
        /// </summary>
        event EventHandler<MessageReceivedEventArgs> MessageReceived;
    }
}
