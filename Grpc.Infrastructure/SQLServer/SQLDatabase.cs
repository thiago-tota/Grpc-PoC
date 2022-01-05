//using PersistenceService.Infrastructure.Model;
//using System.Data.SqlClient;

//namespace PersistenceService.Infrastructure.SQLServer
//{
//    internal class SQLDatabase
//    {
//        private readonly DbCredentials _dbCredentials;
//        private SqlConnection? _sqlConnection;

//        public SQLDatabase(DbCredentials dbCredentials)
//        {
//            _dbCredentials = dbCredentials;
//        }

//        public bool Connect()
//        {
//            var connection = $"Server={_dbCredentials.Host},{_dbCredentials.Port};Database={_dbCredentials.Database};User Id={_dbCredentials.Username};Password={_dbCredentials.Password}";
//            _sqlConnection = new SqlConnection(connection);
//            _sqlConnection.Open();

//            return true;
//        }

//        public bool Disconnect()
//        {
//            _sqlConnection?.Close();

//            return true;
//        }
//    }
//}
