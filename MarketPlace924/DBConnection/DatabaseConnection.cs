using Microsoft.Data.SqlClient;
using System.Threading.Tasks;

namespace MarketPlace924.DBConnection
{
    public class DatabaseConnection
    {
        private SqlConnection _dbConnection = new("Data Source=LAPTOP-MHT5DVFO\\SQLEXPRESS01;Initial Catalog=MarketPlaceDB;Integrated Security=True;TrustServerCertificate=True;");


        public SqlConnection getConnection()
        {
            return _dbConnection;
        }

        public void OpenConnectionSync()
        {
            if (_dbConnection.State == System.Data.ConnectionState.Closed)
                _dbConnection.Open();
        }

        public async Task OpenConnection()
        {
            if (_dbConnection.State == System.Data.ConnectionState.Closed)
                await _dbConnection.OpenAsync();
        }

        public void CloseConnection()
        {
            if (_dbConnection.State == System.Data.ConnectionState.Open)
                _dbConnection.Close();
        }

        public void ExecuteProcedure()
        {
        }
    }
}