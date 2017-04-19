using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apworks.Events
{
    public interface IDomainEventHandler : IEventHandler<IDomainEvent>
    {
    }
}
