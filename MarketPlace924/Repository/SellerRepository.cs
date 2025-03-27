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

        public void LoadSellerInfo(Seller seller)
        {
            _connection.OpenConnectionSync();
            var conn = _connection.getConnection();
            var cmd = conn.CreateCommand();
            cmd.CommandText = @"
            SELECT * FROM Sellers
            WHERE UserId = @UserID";
            cmd.Parameters.AddWithValue("@UserID", seller.Id);
            
            using (var reader = cmd.ExecuteReader())
            {
                if (!reader.Read())
                {
                    _connection.CloseConnection();
                    return;
                }

                seller.StoreName = reader.GetString(reader.GetOrdinal("StoreName"));
                seller.StoreDescription = reader.GetString(reader.GetOrdinal("StoreDescription"));
                seller.StoreAddress = reader.GetString(reader.GetOrdinal("StoreAddress"));
                seller.FollowersCount = reader.GetInt32(reader.GetOrdinal("FollowersCount"));
                seller.TrustScore = reader.GetDouble(reader.GetOrdinal("TrustScore"));
            }
            _connection.CloseConnection();
        }

        public async Task<List<string>?> GetNotificationsAsync(int sellerID)
        {
            await _connection.OpenConnection();
            var command = _connection.getConnection().CreateCommand();
            command.CommandText = "SELECT * FROM Notifications WHERE SellerID = @SellerID";
            command.Parameters.Add(new SqlParameter("@SellerID", sellerID));
            var notifications = new List<string>();
            using (var reader = await command.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    notifications.Add(reader.GetString(2));
                }
            }
            _connection.CloseConnection();
            return notifications;
        }

        public async Task UpdateSellerAsync(Seller seller)
        {
            await _connection.OpenConnection();
            var command = _connection.getConnection().CreateCommand();

            command.CommandText = @"
                    UPDATE Sellers 
                    SET StoreName = @StoreName, StoreDescription = @StoreDescription, StoreAddress = @StoreAddress, FollowersCount = @FollowersCount, TrustScore = @TrustScore 
                    WHERE UserID = @UserID";
            command.Parameters.Add(new SqlParameter("@StoreName", seller.StoreName));
            command.Parameters.Add(new SqlParameter("@StoreDescription", seller.StoreDescription));
            command.Parameters.Add(new SqlParameter("@StoreAddress", seller.StoreAddress));
            command.Parameters.Add(new SqlParameter("@FollowersCount", seller.FollowersCount));
            command.Parameters.Add(new SqlParameter("@TrustScore", seller.TrustScore));
            command.Parameters.Add(new SqlParameter("@UserID", seller.Id));
            await command.ExecuteNonQueryAsync();
            _connection.CloseConnection();
        }


        public async Task<List<Product>?> GetProductsAsync(int sellerID)
        {
            await _connection.OpenConnection();
            var command = _connection.getConnection().CreateCommand();
            command.CommandText = "SELECT * FROM Products WHERE SellerID = @SellerID";
            command.Parameters.Add(new SqlParameter("@SellerID", sellerID));
            var products = new List<Product>();
            using (var reader = await command.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    var productID = reader.GetInt32(0);
                    var productName = reader.GetString(2);
                    var productDescription = reader.GetString(3);
                    var productPrice = reader.GetDouble(4);
                    var productStock = reader.GetInt32(5);
                    products.Add(new Product(productID, productName, productDescription, productPrice, productStock, sellerID));
                }
            }
            _connection.CloseConnection();
            return products;

        }

        public async Task CreateSeller(Seller seller)
        {
            await _connection.OpenConnection();
            var command = _connection.getConnection().CreateCommand();
            command.CommandText = @"INSERT INTO Sellers (UserId, Username, StoreName, StoreDescription, StoreAddress, FollowersCount, TrustScore)
                        VALUES 
                                (@UserId, @Username, @StoreName, @StoreDescription, @StoreAddress, 0, 0)";
            command.Parameters.AddWithValue("@UserId", seller.Id);
            command.Parameters.AddWithValue("@Username", seller.Username);
            command.Parameters.AddWithValue("@StoreName", "");
            command.Parameters.AddWithValue("@StoreDescription", "");
            command.Parameters.AddWithValue("@StoreAddress", "");
            await command.ExecuteNonQueryAsync();
            _connection.CloseConnection();
        }
    }
}
