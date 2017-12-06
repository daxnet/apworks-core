using Apworks.Events;
using Apworks.Messaging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

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

    class NameChangedEventHandler : Events.EventHandler<NameChangedEvent>
    {
        private readonly List<string> names;

        public NameChangedEventHandler(List<string> names)
        {
            this.names = names;
        }

        public override Task<bool> HandleAsync(NameChangedEvent message, CancellationToken cancellationToken = default(CancellationToken))
        {
            this.names.Add(message.Name);
            return Task.FromResult(true);
        }
    }

    class NameChangedEventHandler2 : Events.EventHandler<NameChangedEvent>
    {
        private readonly List<string> names;

        public NameChangedEventHandler2(List<string> names)
        {
            this.names = names;
        }

        public override Task<bool> HandleAsync(NameChangedEvent message, CancellationToken cancellationToken = default(CancellationToken))
        {
            this.names.Add(message.Name + "2");
            return Task.FromResult(true);
        }
    }
}
