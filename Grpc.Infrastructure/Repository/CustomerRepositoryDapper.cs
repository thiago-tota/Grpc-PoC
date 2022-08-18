using Grpc.Domain.Model;

namespace Grpc.Infrastructure.Repository
{
    public class CustomerRepositoryDapper : RepositoryDapperBase<Customer>
    {
        public CustomerRepositoryDapper(string connectionString) : base(connectionString)
        {
        }
    }
}
