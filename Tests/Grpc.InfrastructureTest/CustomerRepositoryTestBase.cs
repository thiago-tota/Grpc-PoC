using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using FluentAssertions;
using Grpc.Domain.Model;
using Grpc.Infrastructure.Repository;
using Xunit;
using Xunit.Abstractions;
using Xunit.Extensions.Ordering;

namespace Grpc.InfrastructureTest
{
    public abstract class CustomerRepositoryTestBase : IClassFixture<CustomerRepositoryFixture>
    {
        protected IRepository<Customer> _customerRepository;
        private readonly ITestOutputHelper _testOutputHelper;

        public CustomerRepositoryTestBase(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public async void GetFirst10Records()
        {
            var watch = new Stopwatch();
            watch.Start();

            var result = await _customerRepository.Get(pageSize: 10);

            result.Should().HaveCount(10);

            watch?.Stop();
            _testOutputHelper.WriteLine($"Elapsed time(ms): {watch?.ElapsedMilliseconds}");
        }

        [Fact]
        public async void GetById()
        {
            object id = 1;

            var result = await _customerRepository.GetById(id);
            result.Should().NotBeNull();
        }

        [Fact, Order(0)]
        public async void CreateNewCustomer()
        {
            var firstCustomer = await _customerRepository.Insert(GetCustomer());
            var secondCustomer = await _customerRepository.Insert(GetCustomer());

            firstCustomer.Should().BeTrue();
            secondCustomer.Should().BeTrue();
        }

        [Fact, Order(1)]
        public async void SearchByName()
        {
            var result = await _customerRepository.Get(filter: c => c.FirstName.Equals("Thiago"));

            result.Should().NotBeNullOrEmpty();
        }

        [Fact, Order(2)]
        public async void UpdateCustomer()
        {

            var customers = await _customerRepository.Get(filter: c => c.FirstName.Equals("Thiago"));
            var customer = customers.First();
            customer.LastName = "De Lima Tota";

            var result = await _customerRepository.Update(customer);

            var refreshedCustomers = await _customerRepository.Get(filter: c => c.FirstName.Equals("Thiago"));
            var refreshedcustomer = refreshedCustomers.First();

            result.Should().BeTrue();
            refreshedcustomer.LastName.Should().Be(customer.LastName);
        }

        [Fact, Order(3)]
        public async void DeleteCustomer()
        {
            var customers = await _customerRepository.Get(filter: c => c.FirstName.Equals("Thiago"));
            var customer = customers.First();

            var result = await _customerRepository.Delete(customer);

            var refreshedCustomers = await _customerRepository.Get(filter: c => c.FirstName.Equals("Thiago"));

            result.Should().BeTrue();
            customers.Should().HaveCountGreaterThan(refreshedCustomers.Count());
        }

        [Fact, Order(4)]
        public async void DeleteCustomerById()
        {
            var customers = await _customerRepository.Get(filter: c => c.FirstName.Equals("Thiago"));

            List<bool> result = new List<bool>();

            foreach (var item in customers)
            {
                result.Add(await _customerRepository.Delete(item.Id));
            }

            var refreshedCustomers = await _customerRepository.Get(filter: c => c.FirstName.Equals("Thiago"));

            result.Should().AllBeEquivalentTo(true);
            refreshedCustomers.Should().BeEmpty();
        }

        #region Performance tests

        [Fact]
        public async void GetFirst500Records1000Times()
        {
            var watch = new Stopwatch();
            watch.Start();

            for (var i = 0; i < 1000; i++)
            {
                var result = await _customerRepository.Get(pageSize: 500);
                if (i == 999)
                    Assert.True(result.Any());
            }

            watch?.Stop();
            _testOutputHelper.WriteLine($"Elapsed time(ms): {watch?.ElapsedMilliseconds}");
        }

        #endregion

        private Customer GetCustomer()
        {
            return new Customer
            {
                FirstName = "Thiago",
                MiddleName = "",
                LastName = "Tota",
                Title = "Mr.",
                Suffix = "",
                SalesPerson = "adventure-works\\garrett1",
                Phone = "394-555-0176",
                EmailAddress = "thiago.delimatota@zuehlke.com",
                CompanyName = "Zühlke",
                PasswordHash = Guid.NewGuid().ToString("N"),
                PasswordSalt = Guid.NewGuid().ToString("N").Substring(0, 10),
                ModifiedDate = DateTime.Now,
                RowGuid = Guid.NewGuid()
            };
        }
    }
}