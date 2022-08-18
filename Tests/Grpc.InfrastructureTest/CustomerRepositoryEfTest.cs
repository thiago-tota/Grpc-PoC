using Grpc.Domain.Model;
using Grpc.Infrastructure.Repository;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Xunit.Abstractions;

namespace Grpc.InfrastructureTest
{
    [UsedImplicitly]
    public class CustomerRepositoryEfTest : CustomerRepositoryTestBase
    {
        public CustomerRepositoryEfTest(IEnumerable<IRepository<Customer>> customerRepositories, ITestOutputHelper testOutputHelper)
            : base(testOutputHelper)
        {
            _customerRepository = customerRepositories.Single(f => f.GetType() == typeof(CustomerRepositoryEf));
        }
    }
}