using MarketPlace924.Domain;
using Microsoft.Data.SqlClient;
using System;
using System.Diagnostics;

namespace MarketPlace924.Repository
{
    class UserRepository
    {
        private DBConnection.DBConnection connection;
        public UserRepository(DBConnection.DBConnection connection)
        {
            this.connection = connection;
        }
        public void addUser(User user)
        {

        }
        public User getUser(string username)
        {
            return new User();
        }
        public void updateUser(User user) { }
        public void deleteUser(string username) { }
        public User? GetUserByEmail(string email)
        {
            var Connection = this.connection.getConnection();
            Connection.Open();
            var command = Connection.CreateCommand();
            command.CommandText = "SELECT * FROM Users WHERE Email = @Email";
            command.Parameters.Add(new SqlParameter("@Email", email));

            Debug.WriteLine("Im in login");
            var reader = command.ExecuteReader();
            if (reader.Read())
            {
                Debug.WriteLine($"UserID: {reader[0]}, Username: {reader[1]}, Email: {reader[2]}, Phone: {reader[3]}, Password: {reader[4]}, Role: {reader[5]}, FailedLogins: {reader[6]}, BannedUntil: {reader[7]} IsBanned:{reader[8]}");
                return new User(
           reader.GetInt32(0), // UserID
           reader.GetString(1), // Username
           reader.GetString(2), // Email
           reader.GetString(3), // PhoneNumber
            reader.GetString(4), // Password (Handle NULL)
           reader.GetInt32(5), // Role
           reader.GetInt32(6), // FailedLogins
           reader.IsDBNull(7) ? (DateTime?)null : reader.GetDateTime(7), // BannedUntil (Handle NULL)
            reader.GetBoolean(8) // IsBanned

       );
                // return new User(reader.GetInt32(0), reader.GetString(1), reader.GetString(2), reader.GetString(3), reader.GetInt32(4), reader.GetInt32(5), reader.GetDateTime(6));
                //return new User(reader.GetInt32(0), reader.GetString(1), reader.GetString(2), reader.GetString(3), reader.GetString(4),reader.GetInt32(5),reader.GetInt32(6),reader.GetDateTime(7));

            }
            return null;
        }
        public bool checkExistanceOfEmail(string email)
        {
            var Connection = this.connection.getConnection();
            Connection.Open();
            var command = Connection.CreateCommand();
            command.CommandText = "SELECT count(1) FROM Users WHERE Email = @Email";
            return (int)command.ExecuteScalar() > 0;
        }
        public bool checkExistanceOfUsername(string username)
        {
            return true;
        }

    }
}
