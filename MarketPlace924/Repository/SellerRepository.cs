using System;
using System.Threading.Tasks;
using MarketPlace924.DBConnection;
using Microsoft.Data.SqlClient;
using MarketPlace924.Domain;
using System.Collections.Generic;


namespace MarketPlace924.Repository
{
    class SellerRepository
    {
        private DatabaseConnection _connection;
        private UserRepository _userRepository;

        public SellerRepository(DatabaseConnection connection, UserRepository userRepository)
        {
            _connection = connection;
            _userRepository = userRepository;
        }

        public Seller? GetSeller(int sellerID)
        {
            _connection.openConnection();
            var command = _connection.getConnection().CreateCommand();
            command.CommandText = "SELECT * FROM Sellers WHERE SellerID = @SellerID";
            command.Parameters.Add(new SqlParameter("@SellerID", sellerID));

            var reader = command.ExecuteReader();
            if (!reader.Read())
            {
                return null;
            }
            var username = reader.GetString(2);
            var storeName = reader.GetString(3);
            var storeDescription = reader.GetString(4);
            var storeAddress = reader.GetString(5);
            var followersCount = reader.GetInt32(6);
            var trustScore = reader.GetFloat(7);

            User? user = _userRepository.GetUserByUsername(username);
            if (user == null)
            {
                return null;
            }

            return new Seller(user, storeName, storeDescription, storeAddress, followersCount, trustScore);
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
            command.CommandText = "UPDATE Sellers SET StoreName = @StoreName, StoreDescription = @StoreDescription, StoreAddress = @StoreAddress, FollowersCount = @FollowersCount, TrustScore = @TrustScore WHERE SellerID = @SellerID";
            command.Parameters.Add(new SqlParameter("@StoreName", seller.StoreName));
            command.Parameters.Add(new SqlParameter("@StoreDescription", seller.StoreDescription));
            command.Parameters.Add(new SqlParameter("@StoreAddress", seller.StoreAddress));
            command.Parameters.Add(new SqlParameter("@FollowersCount", seller.FollowersCount));
            command.Parameters.Add(new SqlParameter("@TrustScore", seller.TrustScore));
            command.Parameters.Add(new SqlParameter("@SellerID", seller.UserId));
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
                var productPrice = reader.GetFloat(4);
                var productStock = reader.GetInt32(7);
                products.Add(new Product(productID, productName, productDescription, productPrice, productStock, sellerID));
            }
            return products;
        }
    }
}
