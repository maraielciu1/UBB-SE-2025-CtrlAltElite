using Microsoft.Data.SqlClient;
using System.Threading.Tasks;

namespace MarketPlace924.DBConnection
{
	public class DatabaseConnection
    {
        // here you can replace the connection string with your connection info from the local database
        private static string dbConnectionString = "Data Source=MELISA-ASUS\\SQLEXPRESS;Initial Catalog=ISS;Integrated Security=True;TrustServerCertificate=True;MultipleActiveResultSets=True;";
        private SqlConnection dbConnection = new SqlConnection(dbConnectionString);

        public DatabaseConnection() { }
        public SqlConnection getConnection()
        {
			return dbConnection;
        }
        public void openConnection()
        {
            if (dbConnection.State == System.Data.ConnectionState.Closed)
                 dbConnection.Open();
        }
        public void closeConnection()
        {
            dbConnection.Close();
        }
        public void executeProcedure()
        {

        }
    }
}
