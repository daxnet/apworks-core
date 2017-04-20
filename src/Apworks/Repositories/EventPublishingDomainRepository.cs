using Apworks.Events;
using Apworks.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apworks.Repositories
{
    public abstract class EventPublishingDomainRepository : DomainRepository
    {
        private readonly IEventPublisher publisher;

        protected EventPublishingDomainRepository(IEventPublisher publisher)
        {
            this.publisher = publisher;
            this.publisher.MessagePublished += OnMessagePublished;
        }

        protected virtual void OnMessagePublished(object sender, MessagePublishedEventArgs e) { }

        protected IEventPublisher Publisher { get; }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.publisher?.Dispose();
            }
        }
    }
}
