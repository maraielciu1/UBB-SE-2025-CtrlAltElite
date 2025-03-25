using Microsoft.Data.SqlClient;
using System.Threading.Tasks;

namespace MarketPlace924.DBConnection
{
	public class DatabaseConnection
    {
        // Connection string to the local SQL Server database and SQL Connection instance
        private static string dbConnectionString = "Data Source=LAPTOP-MHT5DVFO\\SQLEXPRESS01;Initial Catalog=MarketPlaceDB;Integrated Security=True;TrustServerCertificate=True";
        private SqlConnection dbConnection = new SqlConnection(dbConnectionString);

        public DatabaseConnection() { }

        // Gets the current SQL database connection.
        public SqlConnection GetConnection()
        {
			return dbConnection;
        }

        // Opens the database connection asynchronously if it is closed.
        public async Task OpenConnection()
        {
            if(dbConnection.State == System.Data.ConnectionState.Closed)
                await dbConnection.OpenAsync();

        }

        // Closes the database connection.
        public void CloseConnection()
        {
            dbConnection.Close();
        }


        public void ExecuteProcedure()
        {

        }
    }
}
