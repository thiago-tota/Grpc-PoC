using System.Data.SqlClient;

namespace Grpc.Infrastructure.SqlServer
{
    public class SqlDatabase
    {
        private readonly string _connectionString;
        public SqlConnection? SqlConnection;

        public SqlDatabase(string connectionString)
        {
            _connectionString = connectionString;
        }

        public bool Connect()
        {
            SqlConnection = new SqlConnection(_connectionString);
            SqlConnection.Open();

            return true;
        }

        public bool Disconnect()
        {
            SqlConnection?.Close();

            return true;
        }
    }
}
