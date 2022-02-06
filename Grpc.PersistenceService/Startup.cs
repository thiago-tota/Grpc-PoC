using Grpc.Domain.Model;
using Grpc.Infrastructure.SQLServer;
using System.Data.Entity;
using Repo = Grpc.Infrastructure.Repository;

namespace Grpc.PersistenceService
{
    public class Startup
    {
        protected static string ConnectionString { get; set; }

        public void ConfigureServices(IServiceCollection services)
        {
            var configuration = CreateConfiguration();
            ConnectionString = PrepareConnectionString(configuration, "SqlServer");

            RegisterRepositories(services);
        }

        protected virtual void RegisterRepositories(IServiceCollection services)
        {
            services.AddSingleton<DbContext>(p => new AdventureWorksContext(ConnectionString));
            services.AddSingleton<Repo.IRepository<Customer>, Repo.CustomerRepositoryEf>();
            //services.AddSingleton<Repo.IRepository<Customer>>(p => new Repo.CustomerRepository<Customer>(connectionString));
        }

        private static IConfiguration CreateConfiguration()
        {
            return new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("AppSettings.json", false, true)
            .AddEnvironmentVariables()
            .Build();
        }

        private static string PrepareConnectionString(IConfiguration? configuration, string database)
        {
            var connectionString = configuration?.GetValue<string>($"DbCredentials:ConnectionStrings:{database}");

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
