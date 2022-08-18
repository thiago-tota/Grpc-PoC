using System.Data.Entity;
using Grpc.Domain.Model;
using Grpc.Infrastructure.Repository;
using Grpc.Infrastructure.SqlServer;
using Grpc.Service.Settings;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

//Optional
[assembly: CollectionBehavior(DisableTestParallelization = true)]
//Optional
[assembly: TestCaseOrderer("Xunit.Extensions.Ordering.TestCaseOrderer", "Xunit.Extensions.Ordering")]
//Optional
[assembly: TestCollectionOrderer("Xunit.Extensions.Ordering.CollectionOrderer", "Xunit.Extensions.Ordering")]

namespace Grpc.InfrastructureTest
{
    internal class Startup : Grpc.Service.Startup
    {
        protected override void RegisterRepositories(IServiceCollection services, DbSettings dbSettings)
        {

            services.AddSingleton<DbContext>(p => new AdventureWorksContext(dbSettings.ConnectionString));
            services.AddSingleton<IRepository<Customer>, CustomerRepositoryEf>();
            services.AddSingleton<IRepository<Customer>>(p => new CustomerRepositoryDao(dbSettings.ConnectionString));
            services.AddSingleton<IRepository<Customer>>(p => new CustomerRepositoryDapper(dbSettings.ConnectionString));
        }
    }
}
