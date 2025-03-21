using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketPlace924.DBConnection
{
    public class DBConnection
    {
        // here you can replace the connection string with your connection info from the local database
        private static string dbConnectionString = "Data Source=EDINGTON\\SQLEXPRESS;Initial Catalog=dbms_lab1;Integrated Security=True;TrustServerCertificate=True";
        private SqlConnection dbConnection = new SqlConnection(dbConnectionString);

        public DBConnection() { }
        public SqlConnection getConnection()
        {
            //return dbConnection;
            return new SqlConnection(dbConnectionString);
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
