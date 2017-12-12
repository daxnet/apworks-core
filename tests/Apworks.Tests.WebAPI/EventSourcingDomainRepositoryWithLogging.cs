using Apworks.Events;
using Apworks.Repositories;
using Apworks.Snapshots;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Apworks.Tests.WebAPI
{
    public class EventSourcingDomainRepositoryWithLogging : EventSourcingDomainRepository
    {
        private readonly ILogger logger;

        public EventSourcingDomainRepositoryWithLogging(IEventStore eventStore, 
            IEventPublisher publisher, 
            ISnapshotProvider snapshotProvider,
            ILogger<EventSourcingDomainRepositoryWithLogging> logger) 
            : base(eventStore, publisher, snapshotProvider)
        {
            this.logger = logger;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            this.logger.LogInformation($"EventSourcingDomainRepository {this.GetHashCode()} has been disposed.");
        }
    }
}
