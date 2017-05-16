using Apworks.Querying;
using Apworks.Querying.Parsers.Irony;
using Apworks.Tests.Models;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using System.Linq;

namespace Apworks.Tests
{
    public class IronySortSpecificationParserTests
    {
        private readonly List<Customer> Customers = new List<Customer>();

        public IronySortSpecificationParserTests()
        {
            Customers.Add(new Customer { Id = 1, Email = "jim@example.com", Name = "jim" });
            Customers.Add(new Customer { Id = 2, Email = "tom@example.com", Name = "tom" });
            Customers.Add(new Customer { Id = 3, Email = "alex@example.com", Name = "alex" });
            Customers.Add(new Customer { Id = 4, Email = "carol@example.com", Name = "carol" });
            Customers.Add(new Customer { Id = 5, Email = "david@example.com", Name = "david" });
            Customers.Add(new Customer { Id = 6, Email = "frank@example.com", Name = "frank" });
            Customers.Add(new Customer { Id = 7, Email = "peter@example.com", Name = "peter" });
            Customers.Add(new Customer { Id = 8, Email = "paul@example.com", Name = "paul" });
            Customers.Add(new Customer { Id = 9, Email = "winter@example.com", Name = "winter" });
            Customers.Add(new Customer { Id = 10, Email = "julie@example.com", Name = "julie" });
            Customers.Add(new Customer { Id = 11, Email = "jim@example.com", Name = "jim" });
            Customers.Add(new Customer { Id = 12, Email = "brian@example.com", Name = "brian" });
            Customers.Add(new Customer { Id = 13, Email = "david@example.com", Name = "david" });
            Customers.Add(new Customer { Id = 14, Email = "daniel@example.com", Name = "daniel" });
            Customers.Add(new Customer { Id = 15, Email = "jill@example.com", Name = "jill" });
        }

        [Fact]
        public void SortByIdDescendingTest()
        {
            var parser = new IronySortSpecificationParser();
            var spec = parser.Parse<int, Customer>("id d");
            var sorted = Sort(Customers, spec).ToList();
            for (var i = 0; i < sorted.Count; i++)
            {
                Assert.Equal(sorted.Count - i, sorted[i].Id);
            }
        }

        [Fact]
        public void SortByIdAscendingTest()
        {
            var parser = new IronySortSpecificationParser();
            var spec = parser.Parse<int, Customer>("id a");
            var sorted = Sort(Customers, spec).ToList();
            for (var i = 0; i < sorted.Count; i++)
            {
                Assert.Equal(i+1, sorted[i].Id);
            }
        }

        [Fact]
        public void SortByNameAscAndIdDescTest()
        {
            var parser = new IronySortSpecificationParser();
            var spec = parser.Parse<int, Customer>("name d and id a");
            var sorted = Sort(Customers, spec).ToList();
        }

        private static IEnumerable<Customer> Sort(IEnumerable<Customer> origin, SortSpecification<int, Customer> sort)
        {
            var specs = sort.Specifications;
            if (specs.Count() > 0)
            {
                var first = specs.First();
                var result = first.Item2 == SortOrder.Ascending ? origin.OrderBy(first.Item1.Compile()) : origin.OrderByDescending(first.Item1.Compile());
                for(int i=1;i<specs.Count();i++)
                {
                    var spec = specs.ElementAt(i);
                    result = spec.Item2 == SortOrder.Ascending ? result.OrderBy(spec.Item1.Compile()) : result.OrderByDescending(spec.Item1.Compile());
                }
                return result;
            }
            return origin;
        }
    }
}
