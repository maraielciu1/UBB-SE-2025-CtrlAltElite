using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketPlace924.DBConnection
{
    class DBConnection
    {
        // here you can replace the connection string with your connection info from the local database
        private static string dbConnectionString = "Data Source=DESKTOP-618UFK0\\SQLEXPRESS;Initial Catalog=IssDb;Integrated Security=True;TrustServerCertificate=True";
        private SqlConnection dbConnection = new SqlConnection(dbConnectionString);

        public DBConnection() { }
        public SqlConnection getConnection()
        {
            return dbConnection;
        }
        public void openConnection()
        {
            if(dbConnection.State == System.Data.ConnectionState.Closed)
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
