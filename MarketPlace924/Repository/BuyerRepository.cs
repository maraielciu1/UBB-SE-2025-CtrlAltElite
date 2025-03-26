using MarketPlace924.DBConnection;
using MarketPlace924.Domain;
using Microsoft.Data.SqlClient;
using Microsoft.UI.Xaml.Controls.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.System;
using Windows.UI.Notifications;
using Windows.UI.Popups;

namespace MarketPlace924.Repository
{
    public class BuyerRepository
    {
        private DatabaseConnection _connection;

        public BuyerRepository(DatabaseConnection connection)
        {
            _connection = connection;
        }


        // Loads the buyer's followed sellers from the database.
        public async Task LoadBuyerInfo(Buyer buyer)
        {
            await _connection.OpenConnection();

            var command = _connection.GetConnection().CreateCommand();
            command.CommandText = "SELECT FollowedID FROM Following WHERE FollowerID = @FollowerID";
            command.Parameters.AddWithValue("@FollowerID", buyer.Id);

            List<int> sellersIDs = new List<int>();
            using (var reader = await command.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    sellersIDs.Add(Convert.ToInt32(reader["FollowedID"]));
                }
            }
            _connection.CloseConnection();

            buyer.FollowingUsersIds = sellersIDs;
        }


        // Retrieves the list of followed sellers' IDs for a specific buyer.
        public async Task<List<int>> GetFollowingUsersIds(int buyerID)
        {
            List<int> followingUsersIDs = new List<int>();

            await _connection.OpenConnection();

            var command = _connection.GetConnection().CreateCommand();
            command.CommandText = "SELECT FollowedID FROM Following WHERE FollowerID = @FollowerID";
            command.Parameters.AddWithValue("@FollowerID", buyerID);

            using(var reader = await command.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    followingUsersIDs.Add(Convert.ToInt32(reader["FollowedID"]));
                }
            }            

            _connection.CloseConnection();

            return followingUsersIDs;
        }


        // Retrieves the list of followed sellers based on a list of followed user IDs.
        public async Task<List<Seller>> GetFollowedSellers(List<int> followingUsersIds)
        {
            if (followingUsersIds == null || followingUsersIds.Count == 0)
            {
                return new List<Seller>();
            }


            await _connection.OpenConnection();

            var command = _connection.GetConnection().CreateCommand();


            // Add the followed seller IDs to the parameter (using the list of IDs)
            string formattedSellersIds = string.Join(",", followingUsersIds);
            command.CommandText = $"SELECT u.Email, u.PhoneNumber, s.* " +
                                  $"FROM Users u " +
                                  $"INNER JOIN Sellers s " +
                                  $"ON u.UserID = s.SellerID " +
                                  $"WHERE SellerID IN ({formattedSellersIds})";


            List<Seller> followedSellers = new List<Seller>();
            using (var reader = await command.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    var sellerEmail = reader.GetString(0);
                    var sellerPhoneNumber = reader.GetString(1);
                    var sellerID = reader.GetInt32(2);
                    var username = reader.GetString(3);
                    var storeName = reader.GetString(4);
                    var storeDescription = reader.GetString(5);
                    var storeAddress = reader.GetString(6);
                    var followersCount = reader.GetInt32(7);
                    var trustScore = reader.GetDouble(8);

                    Seller seller = new Seller(username, storeName, storeDescription, storeAddress, followersCount, trustScore);
                    Domain.User user = new Domain.User(userID: sellerID, email: sellerEmail, phoneNumber: sellerPhoneNumber);

                    seller.User = user;

                    followedSellers.Add(seller);
                }
            }

            _connection.CloseConnection();
            return followedSellers;
        }

        // Retrieves the list of products from a specific seller.
        public async Task<List<Product>> GetProductsFromSeller(int sellerID)
        {
            await _connection.OpenConnection();

            var command = _connection.GetConnection().CreateCommand();
            command.CommandText = "SELECT * FROM Products WHERE SellerID = @SellerID";
            command.Parameters.AddWithValue("@SellerID", sellerID);

            List<Product> products = new List<Product>();

            using(var reader = await command.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    var productID = reader.GetInt32(0);
                    var productName = reader.GetString(2);
                    var productDescription = reader.GetString(3);
                    var productPrice = reader.GetDouble(4);

                    products.Add(new Product(productID, productName, productDescription, productPrice, 0, sellerID));
                }
            }

            _connection.CloseConnection();
            return products;
        }

        // Check if the Buyer is Following the Seller
        public async Task<bool> IsFollowing(int buyerId, int sellerId)
        {
            await _connection.OpenConnection();
            var command = _connection.GetConnection().CreateCommand();
            command.CommandText = "SELECT COUNT(*) FROM Following WHERE FollowerID = @FollowerID AND FollowedID = @FollowedID";
            command.Parameters.AddWithValue("@FollowerID", buyerId);
            command.Parameters.AddWithValue("@FollowedID", sellerId);

            var result = await command.ExecuteScalarAsync();
            _connection.CloseConnection();

            // Return true if a record exists (i.e., count > 0)
            return Convert.ToInt32(result) > 0;
        }

        // The Buyer Follows the Seller
        public async Task FollowSeller(int buyerId, int sellerId)
        {
            await _connection.OpenConnection();

            var command = _connection.GetConnection().CreateCommand();
            command.CommandText = "INSERT INTO Following (FollowerID, FollowedID) VALUES (@FollowerID, @FollowedID)";
            command.Parameters.AddWithValue("@FollowerID", buyerId);
            command.Parameters.AddWithValue("@FollowedID", sellerId);
            await command.ExecuteNonQueryAsync();

            command = _connection.GetConnection().CreateCommand();
            command.CommandText = "UPDATE Sellers SET FollowersCount = FollowersCount + 1 WHERE SellerID = @SellerID";
            command.Parameters.AddWithValue("@SellerID", sellerId);
            await command.ExecuteNonQueryAsync();

            _connection.CloseConnection();
        }

        // The Buyer Unfollows the Seller
        public async Task UnfollowSeller(int buyerId, int sellerId)
        {
            await _connection.OpenConnection();

            var command = _connection.GetConnection().CreateCommand();
            command.CommandText = "DELETE FROM Following WHERE FollowerID = @FollowerID AND FollowedID = @FollowedID";
            command.Parameters.AddWithValue("@FollowerID", buyerId);
            command.Parameters.AddWithValue("@FollowedID", sellerId);
            await command.ExecuteNonQueryAsync();

            command = _connection.GetConnection().CreateCommand();
            command.CommandText = "UPDATE Sellers SET FollowersCount = FollowersCount - 1 WHERE SellerID = @SellerID";
            command.Parameters.AddWithValue("@SellerID", sellerId);
            await command.ExecuteNonQueryAsync();

            _connection.CloseConnection();
        }
    }
}
