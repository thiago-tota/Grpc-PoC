using Grpc.Domain.Model;
using System.Data.Entity;

namespace Grpc.Infrastructure.SQLServer
{
    public class AdventureWorksContext : DbContext
    {
        public AdventureWorksContext(string connectionString) : base(connectionString)
        {

        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            Database.SetInitializer<AdventureWorksContext>(null);

            modelBuilder.Entity<Customer>().ToTable("Customer", "SalesLT")
                .Property(e => e.CustomerId).HasColumnName("CustomerID");
            modelBuilder.Entity<Customer>().ToTable("Customer", "SalesLT")
               .Property(e => e.ModifiedDate).HasColumnType("datetime");

            base.OnModelCreating(modelBuilder);
        }
    }
}
