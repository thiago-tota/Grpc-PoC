using Grpc.Domain.Model;
using Microsoft.EntityFrameworkCore;

namespace Grpc.Infrastructure.SqlServer
{
    public class AdventureWorksContext : DbContext
    {
        public AdventureWorksContext(DbContextOptions<AdventureWorksContext> options)
            : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Customer>().ToTable("Customer", "SalesLT")
                .Property(e => e.CustomerId).HasColumnName("CustomerID");

            modelBuilder.Entity<Customer>().ToTable("Customer", "SalesLT")
               .Property(e => e.ModifiedDate).HasColumnType("datetime");

            base.OnModelCreating(modelBuilder);
        }
    }
}
