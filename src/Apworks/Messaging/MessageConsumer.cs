using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apworks.Messaging
{
    public abstract class MessageConsumer<TMessageSubscriber> : DisposableObject, IMessageConsumer<TMessageSubscriber>
        where TMessageSubscriber : IMessageSubscriber
    {
        private readonly TMessageSubscriber subscriber;
        private readonly IMessageHandlerExecutionContext messageHandlerManager;
        private bool disposed;

        protected MessageConsumer(TMessageSubscriber subscriber, IMessageHandlerExecutionContext messageHandlerManager)
        {
            this.subscriber = subscriber;
            this.messageHandlerManager = messageHandlerManager;

            this.subscriber.MessageReceived += OnMessageReceived;
            this.subscriber.MessageAcknowledged += OnMessageAcknowledged;
        }

        protected virtual async void OnMessageReceived(object sender, MessageReceivedEventArgs e) 
            => await this.messageHandlerManager?.HandleMessageAsync(e.Message);

        protected virtual async void OnMessageAcknowledged(object sender, MessageAcknowledgedEventArgs e)
        {
            await Task.CompletedTask;
        }

        public TMessageSubscriber Subscriber => subscriber;

        public IMessageHandlerExecutionContext MessageHandlerManager => this.messageHandlerManager;

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
            this.subscriber.Subscribe();
        }
    }
}
