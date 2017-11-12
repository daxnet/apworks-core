using Apworks.Repositories;
using Apworks.Tests.Models;
using System;
using System.Linq;
using Xunit;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Apworks.Querying;
using System.Collections.Generic;
using Apworks.Repositories.Simple;

namespace Apworks.Tests
{
    public class SimpleRepositoryTests : IDisposable
    {
        private readonly IRepositoryContext repositoryContext;
        private readonly IRepository<int, Customer> repository;

        public SimpleRepositoryTests()
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

        [Fact]
        public void UpdateByKeyTest()
        {
            var customers = Customer.CreateMany();
            foreach (var customer in customers)
            {
                this.repository.Add(customer);
            }

            var srcCustomer = customers[4];
            var id = srcCustomer.Id;
            var newCustomer = Customer.CreateOne();

            this.repository.UpdateByKey(id, newCustomer);

            Assert.Equal(newCustomer.Name, this.repository.FindByKey(id).Name);
        }

        [Fact]
        public void Paging1Test()
        {
            var customers = Customer.CreateMany(21);
            foreach (var customer in customers)
            {
                this.repository.Add(customer);
            }

            var idList = customers.OrderBy(x => x.Email).Select(x => x.Id).Skip(5).Take(5).ToList();

            var pagedResult = this.repository.FindAll(new SortSpecification<int, Customer> { { x => x.Email, SortOrder.Ascending } }, 2, 5);

            Assert.Equal(21, pagedResult.TotalRecords);
            Assert.Equal(5, pagedResult.TotalPages);
            Assert.Equal(2, pagedResult.PageNumber);
            Assert.Equal(5, pagedResult.PageSize);
            Assert.True(CompareIds(idList, pagedResult.As<int, Customer>().Select(x => x.Id).ToList()));
        }

        [Fact]
        public void Paging2Test()
        {
            var customers = Customer.CreateMany(1000);
            foreach (var customer in customers)
            {
                this.repository.Add(customer);
            }

            var selectedCustomers = customers.Where(x => x.Id > 20000000).OrderByDescending(x => x.Email);
            var idList = selectedCustomers.Select(x => x.Id).Skip(15).Take(15).ToList();
            var totalRecords = selectedCustomers.Count();
            var totalPages = (totalRecords + 14) / 15;

            var pagedResult = this.repository.FindAll(c => c.Id > 20000000,
                new SortSpecification<int, Customer> { { x => x.Email, SortOrder.Descending } }, 2, 15);

            Assert.Equal(totalRecords, pagedResult.TotalRecords);
            Assert.Equal(totalPages, pagedResult.TotalPages);
            Assert.Equal(2, pagedResult.PageNumber);
            Assert.Equal(15, pagedResult.PageSize);
            Assert.True(CompareIds(idList, pagedResult.As<int, Customer>().Select(x => x.Id).ToList()));
        }

        private static bool CompareIds(List<int> a, List<int> b)
        {
            var ret = true;
            if (a.Count != b.Count)
            {
                ret = false;
            }
            else
            {
                for (var i = 0; i < a.Count; i++)
                {
                    if (a[i] != b[i])
                    {
                        ret = false;
                        break;
                    }
                }
            }

            return ret;
        }
    }
}
