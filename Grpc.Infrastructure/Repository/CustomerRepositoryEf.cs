using Grpc.Domain.Model;
using System.Data.Entity;

namespace Grpc.Infrastructure.Repository
{
    public class CustomerRepositoryEf : RepositoryEfBase<Customer>
    {
        public CustomerRepositoryEf(DbContext context) : base(context)
        {
        }
    }
}
