using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Apworks.Tests.Models
{
    public class Customer : IAggregateRoot<int>
    {
        public int Id { get; set; }

        public string Name { get; set; }
    }
}
