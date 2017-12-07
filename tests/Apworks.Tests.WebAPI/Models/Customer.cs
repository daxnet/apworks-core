using Apworks.Events;
using Apworks.Tests.WebAPI.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Apworks.Tests.WebAPI.Models
{
    public class Customer : AggregateRootWithEventSourcing<Guid>
    {
        public Customer(Guid id)
            : this(id, string.Empty, Address.Empty)
        { }

        public Customer(Guid id, string name)
            : this(id, name, Address.Empty)
        { }

        public Customer(Guid id, string name, Address address)
            : base(id)
        {
            this.SetName(name);
            this.UpdateAddress(address);
        }

        public string Name { get; private set; }

        public Address ContactAddress { get; private set; }

        public void SetName(string name)
        {
            this.Apply<CustomerNameChangedEvent>(new CustomerNameChangedEvent(name));
        }

        public void UpdateAddress(Address newAddress)
        {
            this.Apply<CustomerAddressUpdatedEvent>(CustomerAddressUpdatedEvent.ConstructFromAddress(newAddress));
        }

        [Handles(typeof(CustomerAddressUpdatedEvent))]
        private void HandlesAddressUpdatedEvent(CustomerAddressUpdatedEvent e)
        {
            if (ContactAddress == null)
            {
                ContactAddress = new Address();
            }

            ContactAddress.City = e.City;
            ContactAddress.State = e.State;
            ContactAddress.Country = e.Country;
            ContactAddress.Street = e.Street;
            ContactAddress.ZipCode = e.ZipCode;
        }

        [Handles(typeof(CustomerNameChangedEvent))]
        private void HandlesNameChangedEvent(CustomerNameChangedEvent e)
        {
            this.Name = e.NewName;
        }
    }
}
