using Grpc.Domain;
using System.Linq;
using Xunit;
using Xunit.Abstractions;
using Repo = Grpc.Infrastructure.Repository;

namespace Grpc.InfrastructureTest
{
    public class CustomerRepositoryTest
    {
        readonly Repo.IRepository<Customer> customerRespository;
        readonly ITestOutputHelper testOutputHelper;

        public CustomerRepositoryTest(Repo.IRepository<Customer> customerRepository, ITestOutputHelper testOutputHelper)
        {
            customerRespository = customerRepository;
            this.testOutputHelper = testOutputHelper;
        }

        [Fact]
        public async void CreateNewCustomer()
        {
            //var mock = new Mock<Repo.IRepository<Customer>>();
            //mock.Setup(s => s.Insert(It.Is<Customer>()).Returns(true);
            var customer = new Customer
            {
                FirstName = "Thiago",
                LastName = "Tota",
                EmailAddress = "thiago.delimatota@zuehlke.com",
                CompanyName = "Zühlke"
            };

            var result = await customerRespository.Insert(customer);

            Assert.True(result);
        }

        [Fact]
        public async void GetFirst10Records()
        {
            var result = await customerRespository.Get(pageSize: 10);

            Assert.True(result?.Any());
        }
    }
}