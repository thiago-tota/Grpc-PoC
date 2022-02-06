using Grpc.Domain.Model;
using Grpc.Infrastructure.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Xunit.Abstractions;
using Xunit.Extensions.Ordering;

//Optional
[assembly: CollectionBehavior(DisableTestParallelization = true)]
//Optional
[assembly: TestCaseOrderer("Xunit.Extensions.Ordering.TestCaseOrderer", "Xunit.Extensions.Ordering")]
//Optional
[assembly: TestCollectionOrderer("Xunit.Extensions.Ordering.CollectionOrderer", "Xunit.Extensions.Ordering")]

namespace Grpc.InfrastructureTest
{
    public class CustomerRepositoryTest
    {
        readonly IRepository<Customer> customerRespository;
        readonly ITestOutputHelper testOutputHelper;

        public CustomerRepositoryTest(IEnumerable<IRepository<Customer>> customerRepositories, ITestOutputHelper testOutputHelper)
        {
            customerRespository = customerRepositories.Single(f => f.GetType() == typeof(CustomerRepositoryEf));
            this.testOutputHelper = testOutputHelper;
        }

        [Fact]
        public async void GetFirst10Records()
        {
            var result = await customerRespository.Get(pageSize: 10);

            Assert.True(result.Count() == 10);
        }

        [Fact]
        public async void GetById()
        {
            object id = 1;

            var result = await customerRespository.GetById(id);

            Assert.NotNull(result);
        }

        [Fact, Order(0)]
        public async void CreateNewCustomer()
        {
            var firstCustomer = await customerRespository.Insert(GetCustomer());
            var secondCustomer = await customerRespository.Insert(GetCustomer());

            Assert.True(firstCustomer);
            Assert.True(secondCustomer);
        }

        [Fact, Order(1)]
        public async void SearchByName()
        {
            var result = await customerRespository.Get(filter: c => c.FirstName.Equals("Thiago"));

            Assert.NotEmpty(result);
        }

        [Fact, Order(2)]
        public async void UpdateCustomer()
        {

            var customers = await customerRespository.Get(filter: c => c.FirstName.Equals("Thiago"));
            var customer = customers.First();
            customer.LastName = "De Lima Tota";

            var result = await customerRespository.Update(customer);

            var refreshedCustomers = await customerRespository.Get(filter: c => c.FirstName.Equals("Thiago"));
            var refreshedcustomer = refreshedCustomers.First();

            Assert.True(result);
            Assert.Equal(customer.LastName, refreshedcustomer.LastName);
        }

        [Fact, Order(3)]
        public async void DeleteCustomer()
        {
            var customers = await customerRespository.Get(filter: c => c.FirstName.Equals("Thiago"));
            var customer = customers.First();

            var result = await customerRespository.Delete(customer);

            var refreshedCustomers = await customerRespository.Get(filter: c => c.FirstName.Equals("Thiago"));

            Assert.True(result);
            Assert.True(customers.Count() > refreshedCustomers.Count());
        }

        [Fact, Order(4)]
        public async void DeleteCustomerById()
        {
            var customers = await customerRespository.Get(filter: c => c.FirstName.Equals("Thiago"));

            List<bool> result = new List<bool>();

            foreach (var item in customers)
            {
                result.Add(await customerRespository.Delete(item.Id));
            }

            var refreshedCustomers = await customerRespository.Get(filter: c => c.FirstName.Equals("Thiago"));

            Assert.All(result, r => r.Equals(true));
            Assert.Empty(refreshedCustomers);
        }

        private Customer GetCustomer()
        {
            return new Customer
            {
                FirstName = "Thiago",
                LastName = "Tota",
                EmailAddress = "Thiago.delimatota@zuehlke.com",
                CompanyName = "Zühlke",
                PasswordHash = Guid.NewGuid().ToString("N"),
                PasswordSalt = Guid.NewGuid().ToString("N").Substring(0, 10),
                ModifiedDate = DateTime.Now,
                RowGuid = Guid.NewGuid()
            };
        }
    }
}