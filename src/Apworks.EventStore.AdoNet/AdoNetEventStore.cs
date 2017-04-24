using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Apworks.Events;

namespace Apworks.EventStore.AdoNet
{
    public abstract class AdoNetEventStore : Events.EventStore
    {
        protected override IEnumerable<EventDescriptor> LoadDescriptors<TKey>(string originatorClrType, TKey originatorId)
        {
            throw new NotImplementedException();
        }

        protected override Task<IEnumerable<EventDescriptor>> LoadDescriptorsAsync<TKey>(string originatorClrType, TKey originatorId, CancellationToken cancellationToken = default(CancellationToken))
        {
            return base.LoadDescriptorsAsync(originatorClrType, originatorId, cancellationToken);
        }

        protected override void SaveDescriptors(IEnumerable<EventDescriptor> descriptors)
        {
            throw new NotImplementedException();
        }

        protected override Task SaveDescriptorsAsync(IEnumerable<EventDescriptor> descriptors, CancellationToken cancellationToken = default(CancellationToken))
        {
            return base.SaveDescriptorsAsync(descriptors, cancellationToken);
        }
    }
}
