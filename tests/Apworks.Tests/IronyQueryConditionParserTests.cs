using Apworks.Querying.Parsers.Irony;
using System.Linq;
using System.Collections.Generic;
using Xunit;
using Apworks.Tests.Models;

namespace Apworks.Tests
{
    public class IronyQueryConditionParserTests
    {
        private readonly List<Customer> Customers = new List<Customer>();

        public IronyQueryConditionParserTests()
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
        public void EqualTest()
        {
            var parser = new IronyQueryConditionParser();
            var expression = parser.Parse<Customer>("Name eq \"paul\"");
            var result = Customers.Where(expression.Compile());
            Assert.Equal(1, result.Count());
        }

        [Fact]
        public void NotEqualTest()
        {
            var parser = new IronyQueryConditionParser();
            var expression = parser.Parse<Customer>("Id ne 4");
            var result = Customers.Where(expression.Compile());
            Assert.Equal(14, result.Count());
        }

        [Fact]
        public void NotTest()
        {
            var parser = new IronyQueryConditionParser();
            var expression = parser.Parse<Customer>("not Id eq 4");
            var result = Customers.Where(expression.Compile());
            Assert.Equal(14, result.Count());
        }

        [Fact]
        public void AndTest()
        {
            var parser = new IronyQueryConditionParser();
            var expression = parser.Parse<Customer>("Id ge 5 and Name eq \"jim\"");
            var result = Customers.Where(expression.Compile());
            Assert.Equal(1, result.Count());
        }

        [Fact]
        public void OrTest()
        {
            var parser = new IronyQueryConditionParser();
            var expression = parser.Parse<Customer>("Id ge 5 or Name eq \"jim\"");
            var result = Customers.Where(expression.Compile());
            Assert.Equal(12, result.Count());
        }

        [Fact]
        public void GreatThanOrEqualsToTest()
        {
            var parser = new IronyQueryConditionParser();
            var expression = parser.Parse<Customer>("Id ge 5");
            var result = Customers.Where(expression.Compile());
            Assert.Equal(11, result.Count());
        }

        [Fact]
        public void GreatThanTest()
        {
            var parser = new IronyQueryConditionParser();
            var expression = parser.Parse<Customer>("Id gt 5");
            var result = Customers.Where(expression.Compile());
            Assert.Equal(10, result.Count());
        }

        [Fact]
        public void LessThanOrEqualsToTest()
        {
            var parser = new IronyQueryConditionParser();
            var expression = parser.Parse<Customer>("Id le 5");
            var result = Customers.Where(expression.Compile());
            Assert.Equal(5, result.Count());
        }

        [Fact]
        public void LessThanTest()
        {
            var parser = new IronyQueryConditionParser();
            var expression = parser.Parse<Customer>("Id lt 5");
            var result = Customers.Where(expression.Compile());
            Assert.Equal(4, result.Count());
        }
    }
}
