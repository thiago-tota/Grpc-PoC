using Grpc.Domain;
using System.Data.Entity;

namespace Grpc.Infrastructure.Repository
{
    public class CustomerRepository : BaseRepository<Customer>
    {
        public CustomerRepository(DbContext context) : base(context)
        {
        }
    }
}
