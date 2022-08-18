using System.Collections.Generic;
using System.Linq;
using Grpc.Domain.Model;
using Grpc.Infrastructure.Repository;
using Xunit.Abstractions;

namespace Grpc.InfrastructureTest
{
    public class CustomerRepositoryDapperTest : CustomerRepositoryTestBase
    {
        public CustomerRepositoryDapperTest(IEnumerable<IRepository<Customer>> customerRepositories, ITestOutputHelper testOutputHelper)
            : base(testOutputHelper)
        {
            _customerRepository = customerRepositories.Single(f => f.GetType() == typeof(CustomerRepositoryDapper));
        }
    }
}
