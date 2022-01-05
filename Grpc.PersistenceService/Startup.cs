using Grpc.Domain;
using Grpc.Infrastructure.SQLServer;
using System.Data.Entity;
using Repo = Grpc.Infrastructure.Repository;

namespace Grpc.PersistenceService
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            var configuration = CreateConfiguration();
            var connectionString = PrepareConnectionString(configuration);

            services.AddSingleton<DbContext>(p => new AdventureWorksContext(connectionString));
            services.AddSingleton<Repo.IRepository<Customer>, Repo.CustomerRepository>();
        }

        private static IConfiguration CreateConfiguration()
        {
            return new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("AppSettings.json", false, true)
            .AddEnvironmentVariables()
            .Build();
        }

        private static string PrepareConnectionString(IConfiguration? configuration)
        {
            var connectionString = configuration?.GetValue<string>("DbCredentials:ConnectionStrings:SqlConnection");

            if (connectionString == null)
                throw new ArgumentNullException($"{nameof(connectionString)} cannot be null");

            connectionString = connectionString.Replace("$Host", configuration.GetValue<string>("DbCredentials:SqlServer:Host"));
            connectionString = connectionString.Replace("$Port", configuration.GetValue<string>("DbCredentials:SqlServer:Port"));
            connectionString = connectionString.Replace("$Database", configuration.GetValue<string>("DbCredentials:SqlServer:Database"));
            connectionString = connectionString.Replace("$Username", configuration.GetValue<string>("DbCredentials:SqlServer:Username"));
            connectionString = connectionString.Replace("$Password", configuration.GetValue<string>("DbCredentials:SqlServer:Password"));

            return connectionString;
        }
    }
}
