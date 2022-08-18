using Grpc.Domain.Model;

namespace Grpc.Infrastructure.Repository
{
    public class CustomerRepositoryDao : RepositoryDaoBase<Customer>
    {
        public CustomerRepositoryDao(string connectionString) : base(connectionString)
        {

        }
    }
}
