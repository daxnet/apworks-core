using Apworks.Repositories;
using Apworks.Tests.Models;
using System;
using System.Linq;
using Xunit;
using System.Collections.Concurrent;
using Apworks.Repositories.Dictionary;
using System.Threading.Tasks;

namespace Apworks.Tests
{
    public class DictionaryRepositoryTests : IDisposable
    {
        private readonly IRepositoryContext repositoryContext;
        private readonly IRepository<int, Customer> repository;

        public DictionaryRepositoryTests()
        {
            var session = new ConcurrentDictionary<object, object>();
            this.repositoryContext = new DictionaryRepositoryContext(session);
            this.repository = repositoryContext.GetRepository<int, Customer>();
        }

        public void Dispose()
        {
            this.repositoryContext.Dispose();
        }

        [Fact]
        public void AddTest()
        {
            var customer = Customer.CreateOne();
            this.repository.Add(customer);
            Assert.Equal(1, this.repository.FindAll().Count());
        }

        [Fact]
        public void FindByKeyTest()
        {
            var customers = Customer.CreateMany();
            foreach(var customer in customers)
            {
                this.repository.Add(customer);
            }

            var srcCustomer = customers[3];
            var key = srcCustomer.Id;
            var customer2 = repository.FindByKey(key);
            Assert.True(srcCustomer.Equals(customer2));
        }

        [Fact]
        public async Task FindByKeyAsyncTest()
        {
            var customers = Customer.CreateMany();
            foreach (var customer in customers)
            {
                this.repository.Add(customer);
            }

            var srcCustomer = customers[3];
            var key = srcCustomer.Id;
            var customer2 = await repository.FindByKeyAsync(key);
            Assert.True(srcCustomer.Equals(customer2));
        }

        [Fact]
        public void RemoveByKeyTest()
        {
            var customers = Customer.CreateMany();
            foreach (var customer in customers)
            {
                this.repository.Add(customer);
            }

            var originCount = customers.Count;
            var srcCustomer = customers[3];
            this.repository.RemoveByKey(srcCustomer.Id);
            Assert.Equal(originCount - 1, this.repository.FindAll().Count());
        }

        [Fact]
        public async Task RemoveByKeyAsyncTest()
        {
            var customers = Customer.CreateMany();
            foreach (var customer in customers)
            {
                this.repository.Add(customer);
            }

            var originCount = customers.Count;
            var srcCustomer = customers[3];
            await this.repository.RemoveByKeyAsync(srcCustomer.Id);
            Assert.Equal(originCount - 1, this.repository.FindAll().Count());
        }

        [Fact]
        public void UpdateTest()
        {
            var customers = Customer.CreateMany();
            foreach (var customer in customers)
            {
                this.repository.Add(customer);
            }

            var srcCustomer = customers[5];
            srcCustomer.Name = "daxnet";
            this.repository.Update(srcCustomer);

            Assert.Equal("daxnet", customers[5].Name);
        }

        //[Fact]
        //public void UpdateByKeyTest()
        //{
        //    var customers = Customer.CreateMany();
        //    foreach (var customer in customers)
        //    {
        //        this.repository.Add(customer);
        //    }

        //    var srcCustomer = customers[4];
        //    var newCustomer = Customer.CreateOne(srcCustomer.Id, "daxnet", "email");

        //    this.repository.UpdateByKey(srcCustomer.Id, newCustomer);

        //    Assert.Equal("daxnet", customers[4].Name);
        //}
    }
}
