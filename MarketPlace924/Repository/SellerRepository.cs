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

        public Seller? GetSeller(string sellerUsername)
        {
            _connection.openConnection();
            var command = _connection.getConnection().CreateCommand();
            command.CommandText = "SELECT * FROM Sellers WHERE Username = @Username";
            command.Parameters.Add(new SqlParameter("@Username", sellerUsername));

            using (var reader = command.ExecuteReader())
            {
                if (!reader.Read())
                {
                    return null;
                }
                var username = reader.GetString(1);
                var storeName = reader.GetString(2);
                var storeDescription = reader.GetString(3);
                var storeAddress = reader.GetString(4);
                var followersCount = reader.GetInt32(5);
                var trustScore = reader.GetDouble(6);

                User? user = _userRepository.GetUserByUsername(username);
                if (user == null)
                {
                    return null;
                }

                return new Seller(user, storeName.ToString(), storeDescription.ToString(), storeAddress.ToString(), followersCount, trustScore);

            }
        }

        public int GetSellerIDByUsername(string username)
        {
            _connection.openConnection();
            var command = _connection.getConnection().CreateCommand();
            command.CommandText = "SELECT SellerID FROM Sellers WHERE Username = @Username";
            command.Parameters.Add(new SqlParameter("@Username", username));
            var reader = command.ExecuteReader();
            reader.Read();
            return reader.GetInt32(0);
        }

        public List<String>? GetNotifications(int sellerID)
        {
            _connection.openConnection();
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
            return notifications;
        }

        public void UpdateSeller(Seller seller)
        {
            _connection.openConnection();
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

        public List<Product>? GetProducts(int sellerID)
        {
            _connection.openConnection();
            var command = _connection.getConnection().CreateCommand();
            command.CommandText = "SELECT * FROM Products WHERE SellerID = @SellerID";
            command.Parameters.Add(new SqlParameter("@SellerID", sellerID));
            var reader = command.ExecuteReader();
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
            return products;
        }
    }
}
