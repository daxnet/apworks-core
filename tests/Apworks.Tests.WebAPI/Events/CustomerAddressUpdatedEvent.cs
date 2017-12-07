using Apworks.Events;
using Apworks.Tests.WebAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Apworks.Tests.WebAPI.Events
{
    public class CustomerAddressUpdatedEvent : DomainEvent
    {

        public CustomerAddressUpdatedEvent(string country, string state,
            string city, string street, string zipCode)
        {
            this.Country = country;
            this.State = state;
            this.City = city;
            this.Street = street;
            this.ZipCode = zipCode;
        }

        public static CustomerAddressUpdatedEvent ConstructFromAddress(Address address)
        {
            return new CustomerAddressUpdatedEvent(address.Country, address.State, address.City, address.Street, address.ZipCode);
        }

        public string Country { get; }
        public string State { get; }
        public string City { get; }

        public string Street { get; }

        public string ZipCode { get; }
    }
}
