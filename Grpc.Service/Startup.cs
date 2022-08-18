using Grpc.Domain.Model;
using Grpc.Infrastructure.Repository;
using Grpc.Infrastructure.SqlServer;
using Grpc.Service.Settings;
using Microsoft.EntityFrameworkCore;

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
            if (dbSettings?.ConnectionString == null)
                throw new ArgumentException("ConnectionString cannot be null.");

            var contextOptions = new DbContextOptionsBuilder<AdventureWorksContext>()
                                .UseSqlServer(dbSettings.ConnectionString)
                                .Options;

            services.AddScoped<DbContext>(p => new AdventureWorksContext(contextOptions));
            services.AddSingleton<IRepository<Customer>, CustomerRepositoryEf>();
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
