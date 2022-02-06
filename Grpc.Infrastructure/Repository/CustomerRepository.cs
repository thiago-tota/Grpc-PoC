using ConnectionTest.Infrastructure;
using Grpc.Domain.Model;
using System.Linq.Expressions;

namespace Grpc.Infrastructure.Repository
{
    public class CustomerRepository<T>
        : RepositoryBase<Customer> where T : EntityBase
    {
        private readonly SqlDatabase _sqlDatabase;

        public CustomerRepository(string connectionString)
        {
            _sqlDatabase = new SqlDatabase(connectionString);

            
        }

       
    }
}
