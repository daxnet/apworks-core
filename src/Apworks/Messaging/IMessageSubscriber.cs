using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apworks.Messaging
{
    public interface IMessageSubscriber : IDisposable
    {
        void Subscribe();

        event EventHandler<MessageReceivedEventArgs> MessageReceived;
    }
}
