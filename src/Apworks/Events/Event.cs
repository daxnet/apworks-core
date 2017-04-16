using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apworks.Events
{
    public abstract class Event : IEvent
    {
        public Guid Id { get; set; }

        public DateTime Timestamp { get; set; }
    }
}
