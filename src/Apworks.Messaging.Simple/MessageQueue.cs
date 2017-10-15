using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apworks.Messaging.Simple
{
    internal sealed class MessageQueue
    {
        private readonly IMessageSerializer messageSerializer;
        private readonly ConcurrentQueue<IMessage> queue = new ConcurrentQueue<IMessage>();

        public event EventHandler<MessageProcessedEventArgs> MessagePushed;

        public MessageQueue(IMessageSerializer messageSerializer)
        {
            this.messageSerializer = messageSerializer;
        }

        public void PushMessage(IMessage message)
        {
            queue.Enqueue(message);
            OnMessagePushed(new MessageProcessedEventArgs(message, this.messageSerializer));
        }

        public IMessage PopMessage()
        {
            var succeeded = queue.TryDequeue(out IMessage message);
            return succeeded ? message : null;
        }


        public int Count { get => this.queue.Count; }

        private void OnMessagePushed(MessageProcessedEventArgs e) => this.MessagePushed?.Invoke(this, e);
    }
}
