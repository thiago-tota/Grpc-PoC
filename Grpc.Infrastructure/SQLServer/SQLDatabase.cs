using System.Data.SqlClient;

namespace ConnectionTest.Infrastructure
{
    internal class SqlDatabase
    {
        private readonly string _connectionString;
        private SqlConnection? _sqlConnection;

        public SqlDatabase(string connectionString)
        {
            _connectionString = connectionString;
        }

        public bool Connect()
        {
            _sqlConnection = new SqlConnection(_connectionString);
            _sqlConnection.Open();

            return true;
        }

        public bool Disconnect()
        {
            _sqlConnection?.Close();

            return true;
        }
    }
}
