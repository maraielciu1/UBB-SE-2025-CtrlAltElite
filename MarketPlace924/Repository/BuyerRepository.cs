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
        private DBConnection.DatabaseConnection _connection;

        public BuyerRepository(DatabaseConnection connection)
        {
            _connection = connection;
        }


        // Loads the buyer's info from the database.
        public async Task LoadBuyerInfo(Buyer buyer)
        {
            _connection.OpenConnectionSync();
            var conn = _connection.getConnection();
            var cmd = conn.CreateCommand();
            cmd.CommandText = @"select 
                    FirstName, LastName, Badge, 
                    TotalSpending, NumberOfPurchases, Discount, 
                    ShippingAddressId, BillingAddressId, UseSameAddress
                from Buyers where UserID = @userID";
            cmd.Parameters.AddWithValue("@userID", buyer.Id);
            var reader = cmd.ExecuteReader();
            if (!reader.Read())
            {
                reader.Close();
                return;
            }

            var useSameAddress = reader.GetBoolean(reader.GetOrdinal("UseSameAddress"));
            var billingAddressId = reader.GetInt32(reader.GetOrdinal("BillingAddressId"));
            var shippingAddressId = reader.GetInt32(reader.GetOrdinal("ShippingAddressId"));

            buyer.FirstName = reader.GetString(reader.GetOrdinal("FirstName"));
            buyer.LastName = reader.GetString(reader.GetOrdinal("LastName"));
            buyer.Badge = ParseBadge(reader.GetString(reader.GetOrdinal("Badge")));
            buyer.TotalSpending = reader.GetDecimal(reader.GetOrdinal("TotalSpending"));
            buyer.NumberOfPurchases = reader.GetInt32(reader.GetOrdinal("NumberOfPurchases"));
            buyer.Discount = reader.GetDecimal(reader.GetOrdinal("Discount"));
            buyer.UseSameAddress = useSameAddress;

            reader.Close();

            var billingAddress = LoadAddress(billingAddressId, conn)!;
            buyer.BillingAddress = billingAddress;
            buyer.ShippingAddress = useSameAddress
                ? billingAddress
                : LoadAddress(shippingAddressId, conn)!;


            // FollowingUsersIds For My Market

            var command = _connection.getConnection().CreateCommand();
            command.CommandText = "SELECT FollowedID FROM Following WHERE FollowerID = @FollowerID";
            command.Parameters.AddWithValue("@FollowerID", buyer.Id);

            List<int> sellersIDs = new List<int>();
            using (reader = await command.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    sellersIDs.Add(Convert.ToInt32(reader["FollowedID"]));
                }
            }
            buyer.FollowingUsersIds = sellersIDs;

            _connection.CloseConnection();
        }

        private Address? LoadAddress(int addressId, SqlConnection conn)
        {
            var cmd = conn.CreateCommand();
            cmd.CommandText = @"select StreetLine, City, Country, PostalCode
                            from BuyerAddress where Id = @addressId";
            cmd.Parameters.AddWithValue("@addressId", addressId);

            var reader = cmd.ExecuteReader();
            if (!reader.Read())
            {
                return null;
            }

            var address = new Address
            {
                Id = addressId,
                StreetLine = reader.GetString(reader.GetOrdinal("StreetLine")),
                City = reader.GetString(reader.GetOrdinal("City")),
                Country = reader.GetString(reader.GetOrdinal("Country")),
                PostalCode = reader.GetString(reader.GetOrdinal("PostalCode")),
            };
            reader.Close();
            return address;
        }

        private BuyerBadge ParseBadge(string badgeString)
        {
            return (BuyerBadge)Enum.Parse(typeof(BuyerBadge), badgeString, true);
        }


        public void SaveInfo(Buyer buyer)
        {
            _connection.OpenConnectionSync();
            var conn = _connection.getConnection();
            var command = conn.CreateCommand();
            PersistAddress(buyer.BillingAddress, conn);
            if (!buyer.UseSameAddress)
            {
                PersistAddress(buyer.ShippingAddress, conn);
            }

            command.CommandText = @"UPDATE Buyers 
                                SET 
                                  FirstName = @FirstName,
                                  LastName= @LastName,
                                  BillingAddressId = @BillingAddressId,
                                  UseSameAddress = @UseSameAddress,
                                  ShippingAddressId = @ShippingAddressId
                                WHERE 
                                    UserID = @UserID";

            command.Parameters.AddWithValue("@FirstName", buyer.FirstName);
            command.Parameters.AddWithValue("@LastName", buyer.LastName);
            command.Parameters.AddWithValue("@BillingAddressId", buyer.BillingAddress.Id);
            command.Parameters.AddWithValue("@UseSameAddress", buyer.UseSameAddress);
            command.Parameters.AddWithValue("@ShippingAddressId", buyer.ShippingAddress.Id);
            command.Parameters.Add(new SqlParameter("@UserID", buyer.User.UserId));
            command.ExecuteNonQuery();
        }

        public BuyerWishlist GetWishlist(int userId)
        {
            _connection.OpenConnectionSync();
            var conn = _connection.getConnection();
            var command = conn.CreateCommand();
            command.CommandText = @"SELECT ProductId from BuyerWishlistItems WHERE BuyerId = @userId";
            command.Parameters.AddWithValue("@userId", userId);
            var reader = command.ExecuteReader();
            var wishlist = new BuyerWishlist();
            while (reader.Read())
            {
                wishlist.Items.Add(new BuyerWishlistItem(reader.GetInt32(reader.GetOrdinal("ProductId"))));
            }

            reader.Close();
            return wishlist;
        }

        public List<BuyerLinkage> GetBuyerLinkages(int userId)
        {
            _connection.OpenConnectionSync();
            var conn = _connection.getConnection();
            var command = conn.CreateCommand();
            command.CommandText = @"SELECT RequestingBuyerId, ReceivingBuyerId, IsApproved
                                FROM BuyerLinkage 
                                WHERE RequestingBuyerId = @userId OR  ReceivingBuyerId =@userId";
            command.Parameters.AddWithValue("@userId", userId);
            var reader = command.ExecuteReader();
            var buyerLinkages = new List<BuyerLinkage>();
            while (reader.Read())
            {
                buyerLinkages.Add(ReadBuyerLinkage(reader, userId));
            }

            reader.Close();
            return buyerLinkages;
        }

        public void CreateLinkageRequest(int requestingBuyerId, int receivingBuyerId)
        {
            _connection.OpenConnectionSync();
            var command = _connection.getConnection().CreateCommand();
            command.CommandText = @"INSERT INTO BuyerLinkage(RequestingBuyerId, ReceivingBuyerId, IsApproved)
                                VALUES (@RequestingBuyerId, @ReceivingBuyerId, @IsApproved);";
            command.Parameters.AddWithValue("@RequestingBuyerId", requestingBuyerId);
            command.Parameters.AddWithValue("@ReceivingBuyerId", receivingBuyerId);
            command.Parameters.AddWithValue("@IsApproved", false);

            command.ExecuteNonQuery();
        }
        public void UpdateLinkageRequest(int requestingBuyerId, int receivingBuyerId)
        {
            _connection.OpenConnectionSync();
            var command = _connection.getConnection().CreateCommand();
            command.CommandText = @"UPDATE BuyerLinkage 
                                SET IsApproved=@IsApproved
                                WHERE RequestingBuyerId=@RequestingBuyerId 
                                  AND ReceivingBuyerId=@ReceivingBuyerId;";
            command.Parameters.AddWithValue("@RequestingBuyerId", requestingBuyerId);
            command.Parameters.AddWithValue("@ReceivingBuyerId", receivingBuyerId);
            command.Parameters.AddWithValue("@IsApproved", true);

            command.ExecuteNonQuery();
        }

        public bool DeleteLinkageRequest(int requestingBuyerId, int receivingBuyerId)
        {
            _connection.OpenConnectionSync();
            var command = _connection.getConnection().CreateCommand();
            command.CommandText = @"DELETE FROM BuyerLinkage 
                                WHERE RequestingBuyerId=@RequestingBuyerId
                                  AND ReceivingBuyerId=@ReceivingBuyerId;";
            command.Parameters.AddWithValue("@RequestingBuyerId", requestingBuyerId);
            command.Parameters.AddWithValue("@ReceivingBuyerId", receivingBuyerId);

            return command.ExecuteNonQuery() > 0;
        }

        private BuyerLinkage ReadBuyerLinkage(SqlDataReader reader, int userId)
        {
            var requestingBuyerId = reader.GetInt32(reader.GetOrdinal("RequestingBuyerId"));
            var receivingBuyerId = reader.GetInt32(reader.GetOrdinal("ReceivingBuyerId"));
            var isApproved = reader.GetBoolean(reader.GetOrdinal("IsApproved"));
            var linkedBuyerId = requestingBuyerId;
            var status = BuyerLinkageStatus.Confirmed;

            if (requestingBuyerId == userId)
            {
                linkedBuyerId = receivingBuyerId;
                if (!isApproved)
                {
                    status = BuyerLinkageStatus.PendingOther;
                }
            }
            else
            {
                if (!isApproved)
                {
                    status = BuyerLinkageStatus.PendingSelf;
                }
            }

            var buyerLinkage = new BuyerLinkage
            {
                Buyer = new Buyer
                {
                    User = new Domain.User
                    {
                        UserId = linkedBuyerId
                    }
                },
                Status = status
            };
            return buyerLinkage;
        }


        private void PersistAddress(Address address, SqlConnection conn)
        {
            if (address.Id == 0)
            {
                InsertAddress(address, conn);
            }
            else
            {
                UpdateAddress(address, conn);
            }
        }

        private void UpdateAddress(Address address, SqlConnection conn)
        {
            var cmd = conn.CreateCommand();
            cmd.CommandText = @"UPDATE BuyerAddress
                            SET 
                                StreetLine=@StreetLine,
                                City=@City,
                                Country=@Country,
                                PostalCode=@PostalCode
                            WHERE
                                ID=@ID;";
            cmd.Parameters.AddWithValue("@StreetLine", address.StreetLine);
            cmd.Parameters.AddWithValue("@City", address.City);
            cmd.Parameters.AddWithValue("@Country", address.Country);
            cmd.Parameters.AddWithValue("@PostalCode", address.PostalCode);
            cmd.Parameters.AddWithValue("@ID", address.Id);
            cmd.ExecuteNonQuery();
        }

        private void InsertAddress(Address address, SqlConnection conn)
        {
            var cmd = conn.CreateCommand();
            cmd.CommandText = @"INSERT INTO BuyerAddress(StreetLine,City,Country,PostalCode)
                            Values(@StreetLine, @City, @Country, @PostalCode); SELECT SCOPE_IDENTITY();";
            cmd.Parameters.AddWithValue("@StreetLine", address.StreetLine);
            cmd.Parameters.AddWithValue("@City", address.City);
            cmd.Parameters.AddWithValue("@Country", address.Country);
            cmd.Parameters.AddWithValue("@PostalCode", address.PostalCode);
            address.Id = Convert.ToInt32(cmd.ExecuteScalar());
        }

        public List<Buyer> FindBuyersWithShippingAddress(Address shippingAddress)
        {
            _connection.OpenConnectionSync();
            var command = _connection.getConnection().CreateCommand();
            command.CommandText = @"
                SELECT b.UserId FROM Buyers b
                INNER JOIN BuyerAddress BA on BA.Id = b.ShippingAddressId
                WHERE lower(BA.StreetLine) = lower(@StreetLine) 
                AND lower(BA.City) = lower(@City)
                AND lower(BA.Country) = lower(@Country)
                AND lower(BA.PostalCode) = lower(@PostalCode)
                ";
            command.Parameters.AddWithValue("@StreetLine", shippingAddress.StreetLine);
            command.Parameters.AddWithValue("@City", shippingAddress.City);
            command.Parameters.AddWithValue("@Country", shippingAddress.Country);
            command.Parameters.AddWithValue("@PostalCode", shippingAddress.PostalCode);
            var reader = command.ExecuteReader();
            var buyerIds = new List<int>();
            while (reader.Read())
            {
                buyerIds.Add(reader.GetInt32(reader.GetOrdinal("UserId")));
            }

            reader.Close();
            return buyerIds.Select(buyerId => new Buyer
            {
                User = new Domain.User
                {
                    UserId = buyerId
                }
            }).ToList();
        }

        public void CreateBuyer(Buyer buyer)
        {
            _connection.OpenConnectionSync();
            var conn = _connection.getConnection();
            var command = conn.CreateCommand();
            PersistAddress(buyer.BillingAddress, conn);
            if (!buyer.UseSameAddress)
            {
                PersistAddress(buyer.ShippingAddress, conn);
            }

            command.CommandText = @"INSERT INTO Buyers (UserId, FirstName, LastName, BillingAddressId, ShippingAddressId, UseSameAddress, Badge,
                    TotalSpending, NumberOfPurchases, Discount)
                                VALUES (@UserID, @FirstName, @LastName,  @BillingAddressId, @ShippingAddressId, @UseSameAddress, @Badge, 0, 0,0) ";

            command.Parameters.AddWithValue("@UserId", buyer.Id);
            command.Parameters.AddWithValue("@FirstName", buyer.FirstName);
            command.Parameters.AddWithValue("@LastName", buyer.LastName);
            command.Parameters.AddWithValue("@BillingAddressId", buyer.BillingAddress.Id);
            command.Parameters.AddWithValue("@UseSameAddress", buyer.UseSameAddress);
            command.Parameters.AddWithValue("@ShippingAddressId", buyer.ShippingAddress.Id);
            command.Parameters.AddWithValue("@Badge", BuyerBadge.BRONZE.ToString());
            command.ExecuteNonQuery();
        }


        
        // My Market Functionalities


        // Retrieves the list of followed sellers' IDs for a specific buyer.
        public async Task<List<int>> GetFollowingUsersIds(int buyerID)
        {
            List<int> followingUsersIDs = new List<int>();

            await _connection.OpenConnection();

            var command = _connection.getConnection().CreateCommand();
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

            var command = _connection.getConnection().CreateCommand();


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

            var command = _connection.getConnection().CreateCommand();
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
            var command = _connection.getConnection().CreateCommand();
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

            var command = _connection.getConnection().CreateCommand();
            command.CommandText = "INSERT INTO Following (FollowerID, FollowedID) VALUES (@FollowerID, @FollowedID)";
            command.Parameters.AddWithValue("@FollowerID", buyerId);
            command.Parameters.AddWithValue("@FollowedID", sellerId);
            await command.ExecuteNonQueryAsync();

            command = _connection.getConnection().CreateCommand();
            command.CommandText = "UPDATE Sellers SET FollowersCount = FollowersCount + 1 WHERE SellerID = @SellerID";
            command.Parameters.AddWithValue("@SellerID", sellerId);
            await command.ExecuteNonQueryAsync();

            _connection.CloseConnection();
        }

        // The Buyer Unfollows the Seller
        public async Task UnfollowSeller(int buyerId, int sellerId)
        {
            await _connection.OpenConnection();

            var command = _connection.getConnection().CreateCommand();
            command.CommandText = "DELETE FROM Following WHERE FollowerID = @FollowerID AND FollowedID = @FollowedID";
            command.Parameters.AddWithValue("@FollowerID", buyerId);
            command.Parameters.AddWithValue("@FollowedID", sellerId);
            await command.ExecuteNonQueryAsync();

            command = _connection.getConnection().CreateCommand();
            command.CommandText = "UPDATE Sellers SET FollowersCount = FollowersCount - 1 WHERE SellerID = @SellerID";
            command.Parameters.AddWithValue("@SellerID", sellerId);
            await command.ExecuteNonQueryAsync();

            _connection.CloseConnection();
        }
    }
}

