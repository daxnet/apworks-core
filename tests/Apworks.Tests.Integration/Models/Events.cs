using Apworks.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace Apworks.Tests.Integration.Models
{
    class NameChangedEvent : DomainEvent
    {
        public NameChangedEvent(string name)
        {
            this.Name = name;
        }

        public string Name { get; }
    }

    class TitleChangedEvent : DomainEvent
    {
        public TitleChangedEvent(string title)
        {
            this.Title = title;
        }

        public string Title { get; }
    }

    class RegisteredEvent : DomainEvent
    {

    }
}
