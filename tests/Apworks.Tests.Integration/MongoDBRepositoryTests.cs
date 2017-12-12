using Apworks.Querying;
using Apworks.Repositories.MongoDB;
using Apworks.Tests.Integration.Fixtures;
using Apworks.Tests.Integration.Models;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Xunit;

namespace Apworks.Tests.Integration
{
    public class MongoDBRepositoryTests : IClassFixture<MongoDBFixture>, IDisposable
    {
        private readonly MongoDBFixture fixture;
        private readonly MongoClient mongoClient;

        public MongoDBRepositoryTests(MongoDBFixture fixture)
        {
            Monitor.Enter(MongoDBFixture.locker);
            this.fixture = fixture;
            mongoClient = new MongoClient(fixture.Settings.ClientSettings);
        }

        public void Dispose()
        {
            var database = mongoClient.GetDatabase(fixture.Settings.DatabaseName,
                fixture.Settings.DatabaseSettings);
            var collection = database.GetCollection<Customer>("Customers", fixture.Settings.CollectionSettings);
            collection.DeleteMany(_ => true);
            Monitor.Exit(MongoDBFixture.locker);
        }

        [Fact]
        public void SaveAggregateRootTest()
        {
            using (var repositoryContext = new MongoRepositoryContext(this.fixture.Settings))
            {
                var customer = Customer.CreateOne(1, "Sunny Chen", "daxnet@abc.com");
                var repository = repositoryContext.GetRepository<int, Customer>();
                repository.Add(customer);
                repositoryContext.Commit();

                var customersCount = repository.FindAll().Count();

                Assert.Equal(1, customersCount);
            }
        }

        [Fact]
        public void Save2AggregateRootsTest()
        {
            using (var repositoryContext = new MongoRepositoryContext(this.fixture.Settings))
            {
                var customers = Customer.CreateMany(2);
                var repository = repositoryContext.GetRepository<int, Customer>();
                customers.ToList().ForEach(customer => repository.Add(customer));
                repositoryContext.Commit();

                var customersCount = repository.FindAll().Count();

                Assert.Equal(2, customersCount);
            }
        }

        [Fact]
        public void RemoveByKeyTest()
        {
            using (var repositoryContext = new MongoRepositoryContext(this.fixture.Settings))
            {
                var customer = Customer.CreateOne(1, "Sunny Chen", "daxnet@abc.com");
                var repository = repositoryContext.GetRepository<int, Customer>();
                repository.Add(customer);
                repositoryContext.Commit();

                repository.RemoveByKey(1);
                repositoryContext.Commit();

                var customersCount = repository.FindAll().Count();

                Assert.Equal(0, customersCount);
            }
        }

        [Fact]
        public void UpdateByKeyTest()
        {
            using (var repositoryContext = new MongoRepositoryContext(this.fixture.Settings))
            {
                var customer = Customer.CreateOne(1, "Sunny Chen", "daxnet@abc.com");
                var repository = repositoryContext.GetRepository<int, Customer>();
                repository.Add(customer);
                repositoryContext.Commit();

                customer.Email = "daxnet@123.com";
                repository.UpdateByKey(1, customer);
                repositoryContext.Commit();

                var updatedCustomer = repository.FindByKey(1);

                Assert.Equal("daxnet@123.com", updatedCustomer.Email);
            }
        }

        [Fact]
        public void FindAllTest()
        {
            using (var repositoryContext = new MongoRepositoryContext(this.fixture.Settings))
            {
                var repository = repositoryContext.GetRepository<int, Customer>();
                var customers = Customer.CreateMany(1000);
                foreach(var cust in customers)
                {
                    repository.Add(cust);
                }
                repositoryContext.Commit();

                var result = repository.FindAll();

                Assert.Equal(1000, result.Count());
            }
        }

        [Fact]
        public void FindAllWithPredicateTest()
        {
            using (var repositoryContext = new MongoRepositoryContext(this.fixture.Settings))
            {
                var repository = repositoryContext.GetRepository<int, Customer>();
                var customers = Customer.CreateMany(500);

                foreach (var cust in customers)
                {
                    repository.Add(cust);
                }
                repositoryContext.Commit();

                var subset = customers.Where(x => x.Id > 1000000);

                var result = repository.FindAll(x=>x.Id > 1000000);

                Assert.Equal(subset.Count(), result.Count());
            }
        }

        [Fact]
        public void FindAllWithSortTest()
        {
            using (var repositoryContext = new MongoRepositoryContext(this.fixture.Settings))
            {
                var repository = repositoryContext.GetRepository<int, Customer>();
                var customers = Customer.CreateMany(500);

                foreach (var cust in customers)
                {
                    repository.Add(cust);
                }
                repositoryContext.Commit();

                var sorted = customers.OrderBy(x => x.Email).OrderByDescending(x => x.Name).ToList();

                var result = repository.FindAll(x => true, new SortSpecification<int, Customer> { { _ => _.Email, SortOrder.Ascending }, { _ => _.Name, SortOrder.Descending } }).ToList();

                bool match = true;
                var total = sorted.Count();
                for(var i=0;i<total;i++)
                {
                    if (sorted[i].Email != result[i].Email || sorted[i].Name != result[i].Name)
                    {
                        match = false;
                        break;
                    }
                }

                Assert.True(match);
            }
        }

        [Fact]
        public void PagingTest()
        {
            using (var repositoryContext = new MongoRepositoryContext(this.fixture.Settings))
            {
                var repository = repositoryContext.GetRepository<int, Customer>();
                var customers = Customer.CreateMany(21);
                foreach (var customer in customers)
                {
                    repository.Add(customer);
                }

                var idList = customers.OrderBy(x => x.Email).Select(x => x.Id).Skip(5).Take(5).ToList();

                var pagedResult = repository.FindAll(new SortSpecification<int, Customer> { { x => x.Email, SortOrder.Ascending } }, 2, 5);

                Assert.Equal(21, pagedResult.TotalRecords);
                Assert.Equal(5, pagedResult.TotalPages);
                Assert.Equal(2, pagedResult.PageNumber);
                Assert.Equal(5, pagedResult.PageSize);
                Assert.True(CompareIds(idList, pagedResult.As<int, Customer>().Select(x => x.Id).ToList()));
            }
        }

        [Fact]
        public void Paging2Test()
        {
            using (var repositoryContext = new MongoRepositoryContext(this.fixture.Settings))
            {
                var repository = repositoryContext.GetRepository<int, Customer>();
                var customers = Customer.CreateMany(1000);
                foreach (var customer in customers)
                {
                    repository.Add(customer);
                }

                var selectedCustomers = customers.Where(x => x.Id > 20000000).OrderByDescending(x => x.Email);
                var idList = selectedCustomers.Select(x => x.Id).Skip(15).Take(15).ToList();
                var totalRecords = selectedCustomers.Count();
                var totalPages = (totalRecords + 14) / 15;

                var pagedResult = repository.FindAll(c => c.Id > 20000000,
                    new SortSpecification<int, Customer> { { x => x.Email, SortOrder.Descending } }, 2, 15);

                Assert.Equal(totalRecords, pagedResult.TotalRecords);
                Assert.Equal(totalPages, pagedResult.TotalPages);
                Assert.Equal(2, pagedResult.PageNumber);
                Assert.Equal(15, pagedResult.PageSize);
                Assert.True(CompareIds(idList, pagedResult.As<int, Customer>().Select(x => x.Id).ToList()));
            }
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
