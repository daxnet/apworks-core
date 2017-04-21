using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apworks.Events
{
    public abstract class DomainEvent : Event, IDomainEvent
    {
        public void AttachTo<TKey>(IAggregateRoot<TKey> aggregateRoot)
            where TKey : IEquatable<TKey>
        {
            this.Metadata[EventOriginatorClrTypeMetadataKey] = aggregateRoot.GetType().AssemblyQualifiedName;
            this.Metadata[EventOriginatorIdentifierMetadataKey] = aggregateRoot.Id.ToString();
        }
    }
}
