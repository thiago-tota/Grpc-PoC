using System.Data.Entity;
using Grpc.Domain.Model;
using Grpc.Infrastructure.Repository;
using Grpc.Infrastructure.SqlServer;
using Grpc.Service.Settings;

namespace Grpc.Service
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            var configuration = GetConfiguration();

            var dbSettings = new DbSettings();
            configuration.GetSection(DbSettings.SectionName).Bind(dbSettings);
            RegisterRepositories(services, dbSettings);
        }

        protected virtual void RegisterRepositories(IServiceCollection services, DbSettings dbSettings)
        {
            services.AddSingleton<DbContext>(p => new AdventureWorksContext(dbSettings.ConnectionString));
            services.AddSingleton<IRepository<Customer>, CustomerRepositoryEf>();
            //services.AddSingleton<IRepository<Customer>>(p => new CustomerRepositoryDao(dbSettings.ConnectionString));
            //services.AddSingleton<IRepository<Customer>>(p => new CustomerRepositoryDapper(dbSettings.ConnectionString));
        }

        private static IConfiguration GetConfiguration()
        {
            return new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("AppSettings.json", false, true)
            .AddJsonFile("AppSettings.tests.json", true, true)
            .AddEnvironmentVariables()
            .Build();
        }
    }
}
