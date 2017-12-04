using Apworks.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apworks.Events
{
    [MessageHandlerStub(typeof(IDomainEvent))]
    public interface IDomainEventHandler : IEventHandler<IDomainEvent>
    {
    }
}
