using Grpc.Domain.Model;
using Grpc.Infrastructure.Repository;
using Grpc.Infrastructure.SQLServer;
using Microsoft.Extensions.DependencyInjection;
using System.Data.Entity;

namespace Grpc.InfrastructureTest
{
    internal class Startup : PersistenceService.Startup
    {
        protected override void RegisterRepositories(IServiceCollection services)
        {
            services.AddSingleton<DbContext>(p => new AdventureWorksContext(ConnectionString));
            services.AddSingleton<IRepository<Customer>, CustomerRepositoryEf>();
            services.AddSingleton<IRepository<Customer>>(p => new CustomerRepository<Customer>(ConnectionString));
        }
    }
}
