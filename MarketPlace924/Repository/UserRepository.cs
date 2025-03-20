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
            var Connection = this.connection.getConnection();
            Connection.Open();
            var command = Connection.CreateCommand();
            command.CommandText = "SELECT * FROM Users WHERE Username = @Username";
            command.Parameters.Add(new SqlParameter("@Username", username));

            var reader = command.ExecuteReader();
           
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
               
        }

        public void updateUserFailedLogins(User user,int NewValueOfFailedLogIns)
        {
            var Connection = this.connection.getConnection();
            Connection.Open();
            var command = Connection.CreateCommand();
            command.CommandText = "UPDATE Users SET FailedLogins = @FailedLogins WHERE UserID = @UserID";
            user.FailedLogIns = NewValueOfFailedLogIns;
            command.Parameters.Add(new SqlParameter("@FailedLogins", user.FailedLogIns));
            command.Parameters.Add(new SqlParameter("@UserID", user.UserID));
            command.ExecuteNonQuery();
        }
        public void updateUser(User user) {
            var Connection = this.connection.getConnection();
            Connection.Open();
            var command = Connection.CreateCommand();
            command.CommandText = "UPDATE Users SET Username = @Username, Email = @Email, PhoneNumber = @PhoneNumber, Password = @Password, Role = @Role, FailedLogins = @FailedLogins, BannedUntil = @BannedUntil, IsBanned = @IsBanned WHERE UserID = @UserID";
            command.Parameters.Add(new SqlParameter("@Username", user.Username));
            command.Parameters.Add(new SqlParameter("@Email", user.Email));
            command.Parameters.Add(new SqlParameter("@PhoneNumber", user.PhoneNumber));
            command.Parameters.Add(new SqlParameter("@Password", user.Password));
            command.Parameters.Add(new SqlParameter("@Role", user.Role));
            command.Parameters.Add(new SqlParameter("@FailedLogins", user.FailedLogIns));
            command.Parameters.Add(new SqlParameter("@BannedUntil", user.BannedUntil));
            command.Parameters.Add(new SqlParameter("@IsBanned", user.IsBanned));
            command.Parameters.Add(new SqlParameter("@UserID", user.UserID));
            command.ExecuteNonQuery();

        }
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
            command.Parameters.Add(new SqlParameter("@Email", email));
            return (int)command.ExecuteScalar() > 0;
        }
        public bool checkExistanceOfUsername(string username)
        {
            return true;
        }

        public int getFaildLogInsOfUserByUserID(int userID)
        {
            var Connection = this.connection.getConnection();
            Connection.Open();
            var command = Connection.CreateCommand();
            command.CommandText = "SELECT FailedLogins FROM Users WHERE UserID = @UserID";
            command.Parameters.Add(new SqlParameter("@UserID", userID));
            return (int)command.ExecuteScalar();
        }

    }
}
