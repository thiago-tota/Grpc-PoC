using Grpc.Domain.Model;
using Grpc.Infrastructure.Repository;
using Grpc.Infrastructure.SqlServer;
using Grpc.Service.Settings;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
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
            var contextOptions = new DbContextOptionsBuilder<AdventureWorksContext>()
                                .UseInMemoryDatabase("AdventureWorks")
                                .Options;

            services.AddScoped<DbContext>(p => new AdventureWorksContext(contextOptions));
            services.AddSingleton<IRepository<Customer>, CustomerRepositoryEf>();
            services.AddSingleton<IRepository<Customer>>(p => new CustomerRepositoryDao(dbSettings.ConnectionString));
            services.AddSingleton<IRepository<Customer>>(p => new CustomerRepositoryDapper(dbSettings.ConnectionString));
        }
    }
}
