﻿using MarketPlace924.Domain;
using Microsoft.Data.SqlClient;
using System;
using MarketPlace924.DBConnection;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace MarketPlace924.Repository
{
    public class UserRepository
    {
        private DatabaseConnection _connection;

        public UserRepository(DatabaseConnection connection)
        {
            _connection = connection;
        }

		public async Task AddUser(User user)
		{
			await _connection.OpenConnection();

			var connection = _connection.getConnection();
			var command = connection.CreateCommand();

			command.CommandText = @"
			INSERT INTO Users (Username, Email, PhoneNumber, Password, Role, FailedLogins, BannedUntil, IsBanned)
			VALUES (@Username, @Email, @PhoneNumber, @Password, @Role, @FailedLogins, @BannedUntil, @IsBanned)";

			command.Parameters.AddWithValue("@Username", user.Username);
			command.Parameters.AddWithValue("@Email", user.Email);
			command.Parameters.AddWithValue("@PhoneNumber", user.PhoneNumber);
			command.Parameters.AddWithValue("@Password", user.Password);
			command.Parameters.AddWithValue("@Role", (int)user.Role);
			command.Parameters.AddWithValue("@FailedLogins", user.FailedLogins);
			command.Parameters.AddWithValue("@BannedUntil", user.BannedUntil ?? (object)DBNull.Value);
			command.Parameters.AddWithValue("@IsBanned", user.IsBanned);

			command.ExecuteNonQuery();
			_connection.CloseConnection();
		}

        public async Task<User?> GetUserByUsername(string username)
        {

            await _connection.OpenConnection();
            using var command = _connection.getConnection().CreateCommand();

            command.CommandText = "SELECT * FROM Users WHERE Username = @Username";
            command.Parameters.Add(new SqlParameter("@Username", username));

            using var reader = await command.ExecuteReaderAsync();
            if (!await reader.ReadAsync()) return null;

            var userId = reader.GetInt32(0);
            var email = reader.GetString(2);
            var phoneNumber = reader.GetString(3);
            var password = reader.GetString(4);
            var role = (UserRole)reader.GetInt32(5);
            var failedLoginsCount = reader.GetInt32(6);
            var bannedUntil = reader.IsDBNull(7) ? (DateTime?)null : reader.GetDateTime(7);
            var isBanned = reader.GetBoolean(8);
            _connection.CloseConnection();
            return new User(userId, username, email, phoneNumber, password, role, failedLoginsCount, bannedUntil, isBanned);

        }

        public async Task UpdateUserFailedLoginsCount(User user, int newValueOfFailedLogIns)
		{
			
			await _connection.OpenConnection();
			var command = _connection.getConnection().CreateCommand();

			command.CommandText = "UPDATE Users SET FailedLogins = @FailedLogins WHERE UserID = @UserID";
			user.FailedLogins = newValueOfFailedLogIns;
			command.Parameters.Add(new SqlParameter("@FailedLogins", user.FailedLogins));
			command.Parameters.Add(new SqlParameter("@UserID", user.UserId));
			await command.ExecuteNonQueryAsync();
			_connection.CloseConnection();
        }

		public async Task UpdateUser(User user)
		{
			await _connection.OpenConnection();
			using var command = _connection.getConnection().CreateCommand();

			command.CommandText = "UPDATE Users SET Username = @Username, Email = @Email, PhoneNumber = @PhoneNumber, Password = @Password, Role = @Role, FailedLogins = @FailedLogins, BannedUntil = @BannedUntil, IsBanned = @IsBanned WHERE UserID = @UserID";
			command.Parameters.Add(new SqlParameter("@Username", user.Username));
			command.Parameters.Add(new SqlParameter("@Email", user.Email));
			command.Parameters.Add(new SqlParameter("@PhoneNumber", user.PhoneNumber));
			command.Parameters.Add(new SqlParameter("@Password", user.Password));
			command.Parameters.Add(new SqlParameter("@Role", user.Role));
			command.Parameters.Add(new SqlParameter("@FailedLogins", user.FailedLogins));
			if(user.BannedUntil == null)
				command.Parameters.Add(new SqlParameter("@BannedUntil", DBNull.Value));
			else
				command.Parameters.Add(new SqlParameter("@BannedUntil", user.BannedUntil));
			command.Parameters.Add(new SqlParameter("@IsBanned", user.IsBanned));
			command.Parameters.Add(new SqlParameter("@UserID", user.UserId));
			command.ExecuteNonQuery();
			_connection.CloseConnection();
		}


		public async Task<User?> GetUserByEmail(string email)
		{
			await _connection.OpenConnection();
			var command = _connection.getConnection().CreateCommand();

            command.CommandText = "SELECT * FROM Users WHERE Email = @Email";
            command.Parameters.Add(new SqlParameter("@Email", email));

            var reader = await command.ExecuteReaderAsync();
            if (!await reader.ReadAsync())
            {
                return null;
            }

            var userId = reader.GetInt32(0);
            var username = reader.GetString(1);
            var phoneNumber = reader.GetString(3);
            var password = reader.GetString(4);
            var role = (UserRole)reader.GetInt32(5);
            var failedLoginsCount = reader.GetInt32(6);
            var bannedUntil = reader.IsDBNull(7) ? (DateTime?)null : reader.GetDateTime(7);
            var isBanned = reader.GetBoolean(8);

			await reader.CloseAsync();
			_connection.CloseConnection();
			return new User(userId, username, email, phoneNumber, password, role, failedLoginsCount, bannedUntil, isBanned);
		}
        public async Task<bool> EmailExists(string email)
        {
            await _connection.OpenConnection();
            var command = _connection.getConnection().CreateCommand();

            command.CommandText = "SELECT count(1) FROM Users WHERE Email = @Email";
            command.Parameters.Add(new SqlParameter("@Email", email));
            var result = (int)(await command.ExecuteScalarAsync() ?? 0);
            _connection.CloseConnection();
            return result > 0;
        }

        public async Task<bool> UsernameExists(string username)
        {
            await _connection.OpenConnection();
            var command = _connection.getConnection().CreateCommand();

            command.CommandText = "SELECT COUNT(1) FROM Users WHERE Username = @Username";
            command.Parameters.Add(new SqlParameter("@Username", username));

            var result = (int)(await command.ExecuteScalarAsync() ?? 0);
            return result > 0;
        }

        public async Task<int> GetFailedLoginsCountByUserId(int userId)
        {
            await _connection.OpenConnection();
            var command = _connection.getConnection().CreateCommand();

            command.CommandText = "SELECT FailedLogins FROM Users WHERE UserID = @UserID";
            command.Parameters.Add(new SqlParameter("@UserID", userId));
            var result = (int)(await command.ExecuteScalarAsync() ?? 0);
            _connection.CloseConnection();
            return result;
        }
		


        //region: For Buyer
        public async Task UpdateContactInfo(User user)
        {
	        await _connection.OpenConnection();
	        var conn = _connection.getConnection();
	        var command = conn.CreateCommand();
	        command.CommandText = "UPDATE Users SET PhoneNumber = @PhoneNumber WHERE UserID = @UserID";
            
	        command.Parameters.Add(new SqlParameter("@PhoneNumber", user.PhoneNumber));
	        command.Parameters.Add(new SqlParameter("@UserID", user.UserId));
	        await command.ExecuteNonQueryAsync();
        }

        public async Task LoadUserContactById(User user)
        {
	       await _connection.OpenConnection();
	        var conn = _connection.getConnection();
	        var command = conn.CreateCommand();
	        command.CommandText = "SELECT PhoneNumber, Email FROM Users WHERE UserID = @UserID";
	        command.Parameters.Add(new SqlParameter("@UserID", user.UserId));
	        var reader = await command.ExecuteReaderAsync();
	        if (!await reader.ReadAsync())
	        {
		        return;
	        }
	        user.PhoneNumber = reader.GetString(reader.GetOrdinal("PhoneNumber"));
	        user.Email = reader.GetString(reader.GetOrdinal("Email"));
	       await reader.CloseAsync();
        }

		public async Task<List<User>> GetAll()
		{
			await _connection.OpenConnection();
			var command = _connection.getConnection().CreateCommand();

			command.CommandText = "SELECT * FROM Users";

			var reader = command.ExecuteReader();

			var result = new List<User>();

			while (await reader.ReadAsync())
			{
				var userId = reader.GetInt32(0);
				var username = reader.GetString(1);
				var email = reader.GetString(2);
				var phoneNumber = reader.GetString(3);
				var password = reader.GetString(4);
				var role = (UserRole)reader.GetInt32(5);
				var failedLoginsCount = reader.GetInt32(6);
				var bannedUntil = reader.IsDBNull(7) ? (DateTime?)null : reader.GetDateTime(7);
				var isBanned = reader.GetBoolean(8);

				result.Add(new User(userId, username, email, phoneNumber, password, role, failedLoginsCount, bannedUntil, isBanned));
			}

			await reader.CloseAsync();
			_connection.CloseConnection();
			return result;
		}

		public async Task<int> GetTotalCount()
		{
			await _connection.OpenConnection();
			var command = _connection.getConnection().CreateCommand();

			command.CommandText = "SELECT Count(*) FROM Users";

			var result = (int)command.ExecuteScalar();

			_connection.CloseConnection();
			return result;
		}
		//endregion
	}
}
