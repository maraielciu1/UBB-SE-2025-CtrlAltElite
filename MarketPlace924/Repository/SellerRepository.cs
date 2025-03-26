using System;
using System.Threading.Tasks;
using MarketPlace924.DBConnection;
using Microsoft.Data.SqlClient;
using MarketPlace924.Domain;
using System.Collections.Generic;


namespace MarketPlace924.Repository
{
    public class SellerRepository
    {
        private DatabaseConnection _connection;
        private UserRepository _userRepository;

        public SellerRepository(DatabaseConnection connection, UserRepository userRepository)
        {
            _connection = connection;
            _userRepository = userRepository;
        }

        public async Task<Seller?> GetSellerAsync(string sellerUsername)
        {
            await EnsureConnectionOpenAsync();
            var command = _connection.getConnection().CreateCommand();
            command.CommandText = @"
            SELECT u.UserId, u.Username, u.Email, u.PhoneNumber, u.Password, u.Role, u.FailedLogins, u.BannedUntil, u.IsBanned,
                   s.StoreName, s.StoreDescription, s.StoreAddress, s.FollowersCount, s.TrustScore
            FROM Users u
            INNER JOIN Sellers s ON u.Username = s.Username
            WHERE u.Username = @Username";
            command.Parameters.Add(new SqlParameter("@Username", sellerUsername));

            using (var reader = command.ExecuteReader())
            {
                if (!reader.Read())
                {
                    return null;
                }

                var userId = reader.GetInt32(0);
                var username = reader.GetString(1);
                var email = reader.GetString(2);
                var phoneNumber = reader.GetString(3);
                var password = reader.GetString(4);
                var role = (UserRole)reader.GetInt32(5);
                var failedLogins = reader.GetInt32(6);
                var bannedUntil = reader.IsDBNull(7) ? (DateTime?)null : reader.GetDateTime(7);
                var isBanned = reader.GetBoolean(8);

                var storeName = reader.GetString(9);
                var storeDescription = reader.GetString(10);
                var storeAddress = reader.GetString(11);
                var followersCount = reader.GetInt32(12);
                var trustScore = reader.GetDouble(13);

                var user = new User
                {
                    UserId = userId,
                    Username = username,
                    Email = email,
                    PhoneNumber = phoneNumber,
                    Password = password,
                    Role = role,
                    FailedLogins = failedLogins,
                    BannedUntil = bannedUntil,
                    IsBanned = isBanned
                };

                reader.Close();
                return new Seller(user, storeName, storeDescription, storeAddress, followersCount, trustScore);
            }
        }

        private async Task EnsureConnectionOpenAsync()
        {
            int retryCount = 3;
            while (_connection.getConnection().State != System.Data.ConnectionState.Open && retryCount > 0)
            {
                if (_connection.getConnection().State == System.Data.ConnectionState.Closed)
                {
                    await _connection.openConnection();
                }
                else
                {
                    await Task.Delay(100); // Wait for a short period before retrying
                }
                retryCount--;
            }

            if (_connection.getConnection().State != System.Data.ConnectionState.Open)
            {
                throw new InvalidOperationException("Unable to open the database connection.");
            }
        }




        public async Task<int> GetSellerIDByUsernameAsync(string username)
        {
            await _connection.openConnection();
            var command = _connection.getConnection().CreateCommand();
            command.CommandText = "SELECT SellerID FROM Sellers WHERE Username = @Username";
            command.Parameters.Add(new SqlParameter("@Username", username));
            var reader = command.ExecuteReader();
            reader.Read();
            var read = reader.GetInt32(0);
            reader.Close();
            return read;
        }

        public async Task<List<string>?> GetNotificationsAsync(int sellerID)
        {
            await _connection.openConnection();
            var command = _connection.getConnection().CreateCommand();
            command.CommandText = "SELECT * FROM Notifications WHERE SellerID = @SellerID";
            command.Parameters.Add(new SqlParameter("@SellerID", sellerID));
            var reader = command.ExecuteReader();

            var notifications = new List<string>();
            while (reader.Read())
            {
                notifications.Add(reader.GetString(2));
            }

            _connection.closeConnection();
            reader.Close();
            return notifications;
        }

        public async Task UpdateSellerAsync(Seller seller)
        {
            await _connection.openConnection();
            var command = _connection.getConnection().CreateCommand();
            command.CommandText = "UPDATE Sellers SET StoreName = @StoreName, StoreDescription = @StoreDescription, StoreAddress = @StoreAddress, FollowersCount = @FollowersCount, TrustScore = @TrustScore WHERE Username = @Username";
            command.Parameters.Add(new SqlParameter("@StoreName", seller.StoreName));
            command.Parameters.Add(new SqlParameter("@StoreDescription", seller.StoreDescription));
            command.Parameters.Add(new SqlParameter("@StoreAddress", seller.StoreAddress));
            command.Parameters.Add(new SqlParameter("@FollowersCount", seller.FollowersCount));
            command.Parameters.Add(new SqlParameter("@TrustScore", seller.TrustScore));
            command.Parameters.Add(new SqlParameter("@Username", seller.Username));
            command.ExecuteNonQuery();
        }

        public async Task<List<Product>?> GetProductsAsync(int sellerID)
        {
            await _connection.openConnection();
            var command = _connection.getConnection().CreateCommand();
            command.CommandText = "SELECT * FROM Products WHERE SellerID = @SellerID";
            command.Parameters.Add(new SqlParameter("@SellerID", sellerID));
            using (var reader = command.ExecuteReader())
            {
                var products = new List<Product>();
                while (reader.Read())
                {
                    var productID = reader.GetInt32(0);
                    var productName = reader.GetString(2);
                    var productDescription = reader.GetString(3);
                    var productPrice = reader.GetDouble(4);
                    var productStock = reader.GetInt32(5);
                    products.Add(new Product(productID, productName, productDescription, productPrice, productStock, sellerID));
                }
                reader.Close();
                return products;
            }
        }
    }
}
