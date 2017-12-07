using Apworks.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Apworks.Tests.WebAPI.Events
{
    public class CustomerNameChangedEvent : DomainEvent
    {
        public CustomerNameChangedEvent(string newName)
        {
            this.NewName = newName;
        }

        public string NewName { get; }
    }
}
