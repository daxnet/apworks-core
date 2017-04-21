using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apworks.Events
{
    public sealed class EventDescriptor : IAggregateRoot<Guid>
    {
        public Guid Id { get; set; }

        public Guid EventId { get; set; }

        public DateTime EventTimestamp { get; set; }

        public string EventClrType { get; set; }

        public string EventIntent { get; set; }

        public string OriginatorClrType { get; set; }

        public string OriginatorId { get; set; }

        public object EventPayload { get; set; }
    }
}
