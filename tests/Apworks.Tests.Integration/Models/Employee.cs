using Apworks.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace Apworks.Tests.Integration.Models
{
    public class Employee : AggregateRootWithEventSourcing<Guid>
    {
        public string Name { get; private set; }

        public string Title { get; private set; }

        public DateTime DateRegistered { get; private set; }

        public void ChangeName(string name)
        {
            this.Apply<NameChangedEvent>(new NameChangedEvent(name));
        }

        public void ChangeTitle(string title)
        {
            this.Apply<TitleChangedEvent>(new TitleChangedEvent(title));
        }

        public void Register()
        {
            this.Apply<RegisteredEvent>();
        }

        [Handles(typeof(TitleChangedEvent))]
        private void HandlesTitleChangedEvent(TitleChangedEvent evnt)
        {
            this.Title = $"Sr. {evnt.Title}";
        }

        [Handles(typeof(RegisteredEvent))]
        private void HandlesRegisteredEvent(RegisteredEvent evnt)
        {
            this.DateRegistered = DateTime.UtcNow;
        }
    }
}
