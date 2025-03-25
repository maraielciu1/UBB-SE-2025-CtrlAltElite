using MarketPlace924.Domain;
using Microsoft.Data.SqlClient;
using System;
using MarketPlace924.DBConnection;
using System.Threading.Tasks;

namespace MarketPlace924.Repository
{
	public class UserRepository
	{
		private DatabaseConnection _connection;

		public UserRepository(DatabaseConnection connection)
		{
			_connection = connection;
		}

		public void AddUser(User user)
		{

		}

		public async Task<User?> GetUserByUsername(string username)
		{
			// this needs to be awaited
			_connection.OpenConnection();
			var command = _connection.GetConnection().CreateCommand();

			command.CommandText = "SELECT * FROM Users WHERE Username = @Username";
			command.Parameters.Add(new SqlParameter("@Username", username));


			using (var reader = await command.ExecuteReaderAsync())
			{
                if (!await reader.ReadAsync())
                {
                    return null;
                }

                var userId = reader.GetInt32(0);
                var email = reader.GetString(2);
                var phoneNumber = reader.GetString(3);
                var password = reader.GetString(4);
                var role = (UserRole)reader.GetInt32(5);
                var failedLoginsCount = reader.GetInt32(6);
                var bannedUntil = reader.IsDBNull(7) ? (DateTime?)null : reader.GetDateTime(7);
                var isBanned = reader.GetBoolean(8);

                return new User(userId, username, email, phoneNumber, password, role, failedLoginsCount, bannedUntil, isBanned);
            }


        }

		public void UpdateUserFailedLoginsCount(User user, int NewValueOfFailedLogIns)
		{
			// this needs to be awaited
			_connection.OpenConnection();
			var command = _connection.GetConnection().CreateCommand();

			command.CommandText = "UPDATE Users SET FailedLogins = @FailedLogins WHERE UserID = @UserID";
			user.FailedLogins = NewValueOfFailedLogIns;
			command.Parameters.Add(new SqlParameter("@FailedLogins", user.FailedLogins));
			command.Parameters.Add(new SqlParameter("@UserID", user.UserId));
			command.ExecuteNonQuery();
		}

		public void UpdateUser(User user)
		{
			// this needs to be awaited
			_connection.OpenConnection();
			var command = _connection.GetConnection().CreateCommand();

			command.CommandText = "UPDATE Users SET Username = @Username, Email = @Email, PhoneNumber = @PhoneNumber, Password = @Password, Role = @Role, FailedLogins = @FailedLogins, BannedUntil = @BannedUntil, IsBanned = @IsBanned WHERE UserID = @UserID";
			command.Parameters.Add(new SqlParameter("@Username", user.Username));
			command.Parameters.Add(new SqlParameter("@Email", user.Email));
			command.Parameters.Add(new SqlParameter("@PhoneNumber", user.PhoneNumber));
			command.Parameters.Add(new SqlParameter("@Password", user.Password));
			command.Parameters.Add(new SqlParameter("@Role", user.Role));
			command.Parameters.Add(new SqlParameter("@FailedLogins", user.FailedLogins));
			command.Parameters.Add(new SqlParameter("@BannedUntil", user.BannedUntil));
			command.Parameters.Add(new SqlParameter("@IsBanned", user.IsBanned));
			command.Parameters.Add(new SqlParameter("@UserID", user.UserId));
			command.ExecuteNonQuery();
		}

		public void DeleteUser(string username) { }

		public async Task<User?> GetUserByEmail(string email)
		{
			await _connection.OpenConnection();
			var command = _connection.GetConnection().CreateCommand();

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

			return new User(userId, username, email, phoneNumber, password, role, failedLoginsCount, bannedUntil, isBanned);
		}
		public bool EmailExists(string email)
		{
			// this needs to be awaited
			_connection.OpenConnection();
			var command = _connection.GetConnection().CreateCommand();

			command.CommandText = "SELECT count(1) FROM Users WHERE Email = @Email";
			command.Parameters.Add(new SqlParameter("@Email", email));
			return (int)command.ExecuteScalar() > 0;
		}
		public bool UsernameExists(string username)
		{
			return true;
		}

		public int GetFailedLoginsCountByUserId(int userID)
		{
			// this needs to be awaited
			_connection.OpenConnection();
			var command = _connection.GetConnection().CreateCommand();

			command.CommandText = "SELECT FailedLogins FROM Users WHERE UserID = @UserID";
			command.Parameters.Add(new SqlParameter("@UserID", userID));
			return (int)command.ExecuteScalar();
		}
	}
}
