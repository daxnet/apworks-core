using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apworks.Events
{
    public abstract class DomainEvent : Event, IDomainEvent
    {
        public const string EventAggregateRootClrTypeMetadataKey = "apworks:domainevent.aggregate.clrtype";
        public const string EventAggregateRootIdStringRepresentationMetadataKey = "apworks:domainevent.aggregate.idstr";

        public string GetAggregateRootClrType() => this.Metadata[EventClrTypeMetadataKey].ToString();

        public string GetAggregateRootIdStringRepresentation() => this.Metadata[EventAggregateRootIdStringRepresentationMetadataKey].ToString();

        public void AttachTo<TKey>(IAggregateRoot<TKey> aggregateRoot)
            where TKey : IEquatable<TKey>
        {
            this.Metadata.Add(EventAggregateRootClrTypeMetadataKey, aggregateRoot.GetType().AssemblyQualifiedName);
            this.Metadata.Add(EventAggregateRootIdStringRepresentationMetadataKey, aggregateRoot.Id.ToString());
        }

        public override EventDescriptor ToDescriptor()
        {
            var descriptor = base.ToDescriptor();
            descriptor.OriginatorClrType = this.GetAggregateRootClrType();
            descriptor.OriginatorId = this.GetAggregateRootIdStringRepresentation();
            return descriptor;
        }
    }
}
