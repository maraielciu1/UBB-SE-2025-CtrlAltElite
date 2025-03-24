using Microsoft.Data.SqlClient;
using System.Threading.Tasks;

namespace MarketPlace924.DBConnection
{
	public class DatabaseConnection
    {
        // here you can replace the connection string with your connection info from the local database
        private static readonly string dbConnectionString = "Server=MARA-DELL\\SQLEXPRESS01;Initial Catalog=IssDB;Integrated Security=True;TrustServerCertificate=True";
        private readonly SqlConnection dbConnection = new(dbConnectionString);

        public DatabaseConnection() { }
        public SqlConnection getConnection()
        {
			return dbConnection;
        }
        public async Task OpenConnection()
        {
            if(dbConnection.State == System.Data.ConnectionState.Closed)
                await dbConnection.OpenAsync();
        }
        public void CloseConnection()
        {
            if (dbConnection.State == System.Data.ConnectionState.Open)
                dbConnection.Close();
        }
        public void ExecuteProcedure()
        {

        }
    }
}
