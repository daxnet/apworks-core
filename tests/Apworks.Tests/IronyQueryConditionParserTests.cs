using Apworks.Querying.Parsers.Irony;
using System.Linq;
using System.Collections.Generic;
using Xunit;
using Apworks.Tests.Models;
using System;

namespace Apworks.Tests
{
    public class IronyQueryConditionParserTests
    {
        private readonly List<Customer> Customers = new List<Customer>();
        private static readonly DateTime CurrentDate = new DateTime(2017, 5, 17);

        public IronyQueryConditionParserTests()
        {
            Customers.Add(new Customer { Id = 1, Email = "jim@example.com", Name = "jim", DateRegistered = DateTime.Now.AddDays(-1) });
            Customers.Add(new Customer { Id = 2, Email = "tom@example.com", Name = "tom", DateRegistered = DateTime.Now.AddDays(-2) });
            Customers.Add(new Customer { Id = 3, Email = "alex@example.com", Name = "alex", DateRegistered = DateTime.Now.AddDays(-3) });
            Customers.Add(new Customer { Id = 4, Email = "carol@example.com", Name = "carol", DateRegistered = DateTime.Now.AddDays(-4) });
            Customers.Add(new Customer { Id = 5, Email = "david@example.com", Name = "david", DateRegistered = DateTime.Now.AddDays(-5) });
            Customers.Add(new Customer { Id = 6, Email = "frank@example.com", Name = "frank", DateRegistered = DateTime.Now.AddDays(-6) });
            Customers.Add(new Customer { Id = 7, Email = "peter@example.com", Name = "peter", DateRegistered = DateTime.Now.AddDays(-7) });
            Customers.Add(new Customer { Id = 8, Email = "paul@example.com", Name = "paul", DateRegistered = DateTime.Now.AddDays(1) });
            Customers.Add(new Customer { Id = 9, Email = "winter@example.com", Name = "winter", DateRegistered = DateTime.Now.AddDays(2) });
            Customers.Add(new Customer { Id = 10, Email = "julie@example.com", Name = "julie", DateRegistered = DateTime.Now.AddDays(3) });
            Customers.Add(new Customer { Id = 11, Email = "jim@example.com", Name = "jim", DateRegistered = DateTime.Now.AddDays(4) });
            Customers.Add(new Customer { Id = 12, Email = "brian@example.com", Name = "brian", DateRegistered = DateTime.Now.AddDays(5) });
            Customers.Add(new Customer { Id = 13, Email = "david@example.com", Name = "david", DateRegistered = DateTime.Now.AddDays(6) });
            Customers.Add(new Customer { Id = 14, Email = "daniel@example.com", Name = "daniel", DateRegistered = DateTime.Now.AddDays(7) });
            Customers.Add(new Customer { Id = 15, Email = "jill@example.com", Name = "jill", DateRegistered = DateTime.Now.AddDays(8) });
        }

        [Fact]
        public void EqualTest()
        {
            var parser = new IronyQueryConditionParser();
            var expression = parser.Parse<Customer>("Name eq \"paul\"");
            var result = Customers.Where(expression.Compile());
            Assert.Single(result);
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
            Assert.Single(result);
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

        [Fact]
        public void StringStartsWithTest()
        {
            var parser = new IronyQueryConditionParser();
            var expression = parser.Parse<Customer>("email sw \"fr\"");
            var result = Customers.Where(expression.Compile());
            Assert.Single(result);
        }

        [Fact]
        public void StringEndsWithTest()
        {
            var parser = new IronyQueryConditionParser();
            var expression = parser.Parse<Customer>("name ew \"er\"");
            var result = Customers.Where(expression.Compile());
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public void StringStartsEndsWithCombinedTest()
        {
            var parser = new IronyQueryConditionParser();
            var expression = parser.Parse<Customer>("name ew \"er\" and email sw \"w\"");
            var result = Customers.Where(expression.Compile());
            Assert.Single(result);
        }

        [Fact]
        public void StringContainsTest()
        {
            var parser = new IronyQueryConditionParser();
            var expression = parser.Parse<Customer>("name ct \"ul\"");
            var result = Customers.Where(expression.Compile());
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public void DatePropertyLessThanTest()
        {
            var today = DateTime.Now.ToString("MM/dd/yyyy");
            var parser = new IronyQueryConditionParser();
            var expression = parser.Parse<Customer>($"dateRegistered lt \"{today}\"");
            var result = Customers.Where(expression.Compile());
            Assert.Equal(7, result.Count());
        }

        [Fact]
        public void DatePropertyGreatThanTest()
        {
            var today = DateTime.Now.ToString("MM/dd/yyyy");
            var parser = new IronyQueryConditionParser();
            var expression = parser.Parse<Customer>($"dateRegistered gt \"{today}\"");
            var result = Customers.Where(expression.Compile());
            Assert.Equal(8, result.Count());
        }

        [Fact]
        public void CombinedQueryTest()
        {
            var today = DateTime.Now.ToString("MM/dd/yyyy");
            var parser = new IronyQueryConditionParser();
            var queryString = $"(Name EQ \"jim\" or Name SW \"da\") AND NOT DateRegistered LT \"{today}\"";
            var expression = parser.Parse<Customer>(queryString);
            var result = Customers.Where(expression.Compile());
            Assert.Equal(3, result.Count());
        }
    }
}
