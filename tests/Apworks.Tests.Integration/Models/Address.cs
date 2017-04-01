using System;
using System.Collections.Generic;
using System.Text;

namespace Apworks.Tests.Integration.Models
{
    public class Address : IEntity<int>
    {
        public string Country { get; set; }

        public string State { get; set; }

        public string City { get; set; }

        public string Street { get; set; }

        public string ZipCode { get; set; }
        public int Id { get; set; }
    }
}
