using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apworks.Messaging
{
    public abstract class MessageConsumer<TMessageSubscriber, TMessageHandler> : DisposableObject, IMessageConsumer<TMessageSubscriber, TMessageHandler>
        where TMessageSubscriber : IMessageSubscriber
        where TMessageHandler : IMessageHandler
    {
        private readonly TMessageSubscriber subscriber;
        private readonly IEnumerable<TMessageHandler> handlers;
        private readonly string route;
        private bool disposed;

        protected MessageConsumer(TMessageSubscriber subscriber, IEnumerable<TMessageHandler> handlers, string route = null)
        {
            this.subscriber = subscriber;
            this.handlers = handlers;
            this.route = route;

            this.subscriber.MessageReceived += OnMessageReceived;
            this.subscriber.MessageAcknowledged += OnMessageAcknowledged;
        }

        protected virtual async void OnMessageReceived(object sender, MessageReceivedEventArgs e)
        {
            if (this.handlers != null)
            {
                var messageType = e.Message.GetType();
                foreach(var handler in this.handlers)
                {
                    if (handler.CanHandle(messageType))
                    {
                        await handler.HandleAsync(e.Message);
                    }
                }
            }
        }

        protected virtual async void OnMessageAcknowledged(object sender, MessageAcknowledgedEventArgs e)
        {
            await Task.CompletedTask;
        }

        public TMessageSubscriber Subscriber => subscriber;

        public IEnumerable<TMessageHandler> Handlers => handlers;

        protected override void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    if (this.subscriber != null)
                    {
                        this.subscriber.MessageReceived -= this.OnMessageReceived;
                        this.subscriber.MessageAcknowledged -= this.OnMessageAcknowledged;
                        this.subscriber.Dispose();
                    }
                }

                disposed = true;
                base.Dispose(disposing);
            }
        }

        public void Consume()
        {
            this.subscriber.Subscribe(this.route);
        }
    }
}
