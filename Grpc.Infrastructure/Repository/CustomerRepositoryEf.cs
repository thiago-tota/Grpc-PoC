using Grpc.Domain.Model;
using Microsoft.EntityFrameworkCore;

namespace Grpc.Infrastructure.Repository
{
    public class CustomerRepositoryEf : RepositoryEfBase<Customer>
    {
        public CustomerRepositoryEf(DbContext context) : base(context) { }
    }
}
