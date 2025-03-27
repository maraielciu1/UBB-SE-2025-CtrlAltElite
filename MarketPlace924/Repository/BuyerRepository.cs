using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MarketPlace924.DBConnection;
using MarketPlace924.Domain;
using Microsoft.Data.SqlClient;

namespace MarketPlace924.Repository
{
    public class BuyerRepository
    {
        private DatabaseConnection _connection;

        public BuyerRepository(DatabaseConnection connection)
        {
            _connection = connection;
        }


        // Loads the buyer's info from the database.
        public async Task LoadBuyerInfo(Buyer buyer)
        {
            await _connection.OpenConnection();
            var conn = _connection.getConnection();
            var cmd = conn.CreateCommand();
            cmd.CommandText = @"select 
                    FirstName, LastName, Badge, 
                    TotalSpending, NumberOfPurchases, Discount, 
                    ShippingAddressId, BillingAddressId, UseSameAddress
                from Buyers where UserID = @userID";
            cmd.Parameters.AddWithValue("@userID", buyer.Id);
            var reader = await cmd.ExecuteReaderAsync();
            if (!await reader.ReadAsync())
            {
                await reader.CloseAsync();
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

            await reader.CloseAsync();

            var billingAddress = (await LoadAddress(billingAddressId, conn))!;
            buyer.BillingAddress = billingAddress;
            buyer.ShippingAddress = useSameAddress
                ? billingAddress
                : (await LoadAddress(shippingAddressId, conn))!;


            // FollowingUsersIds For My Market

            var command = _connection.getConnection().CreateCommand();
            command.CommandText = "SELECT FollowedID FROM Following WHERE FollowerID = @FollowerID";
            command.Parameters.AddWithValue("@FollowerID", buyer.Id);

            List<int> sellersIDs = new List<int>();
            using (reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    sellersIDs.Add(Convert.ToInt32(reader["FollowedID"]));
                }
            }

            buyer.FollowingUsersIds = sellersIDs;

            _connection.CloseConnection();
        }

        private async Task<Address?> LoadAddress(int addressId, SqlConnection conn)
        {
            var cmd = conn.CreateCommand();
            cmd.CommandText = @"select StreetLine, City, Country, PostalCode
                            from BuyerAddress where Id = @addressId";
            cmd.Parameters.AddWithValue("@addressId", addressId);

            var reader = await cmd.ExecuteReaderAsync();
            if (!await reader.ReadAsync())
            {
                await reader.CloseAsync();
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
            await reader.CloseAsync();
            return address;
        }

        private BuyerBadge ParseBadge(string badgeString)
        {
            return (BuyerBadge)Enum.Parse(typeof(BuyerBadge), badgeString, true);
        }


        public async Task SaveInfo(Buyer buyer)
        {
            await _connection.OpenConnection();
            var conn = _connection.getConnection();
            var command = conn.CreateCommand();
            await PersistAddress(buyer.BillingAddress, conn);
            if (!buyer.UseSameAddress)
            {
                await PersistAddress(buyer.ShippingAddress, conn);
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
            await command.ExecuteNonQueryAsync();
        }

        public async Task<BuyerWishlist> GetWishlist(int userId)
        {
            await _connection.OpenConnection();
            var conn = _connection.getConnection();
            var command = conn.CreateCommand();
            command.CommandText = @"SELECT ProductId from BuyerWishlistItems WHERE BuyerId = @userId";
            command.Parameters.AddWithValue("@userId", userId);
            var reader = await command.ExecuteReaderAsync();
            var wishlist = new BuyerWishlist();
            while (await reader.ReadAsync())
            {
                wishlist.Items.Add(new BuyerWishlistItem(reader.GetInt32(reader.GetOrdinal("ProductId"))));
            }

            await reader.CloseAsync();
            return wishlist;
        }

        public async Task<List<BuyerLinkage>> GetBuyerLinkages(int userId)
        {
            await _connection.OpenConnection();
            var conn = _connection.getConnection();
            var command = conn.CreateCommand();
            command.CommandText = @"SELECT RequestingBuyerId, ReceivingBuyerId, IsApproved
                                FROM BuyerLinkage 
                                WHERE RequestingBuyerId = @userId OR  ReceivingBuyerId =@userId";
            command.Parameters.AddWithValue("@userId", userId);
            var reader = await command.ExecuteReaderAsync();
            var buyerLinkages = new List<BuyerLinkage>();
            while (await reader.ReadAsync())
            {
                buyerLinkages.Add(ReadBuyerLinkage(reader, userId));
            }

            await reader.CloseAsync();
            return buyerLinkages;
        }

        public async Task CreateLinkageRequest(int requestingBuyerId, int receivingBuyerId)
        {
            await _connection.OpenConnection();
            var command = _connection.getConnection().CreateCommand();
            command.CommandText = @"INSERT INTO BuyerLinkage(RequestingBuyerId, ReceivingBuyerId, IsApproved)
                                VALUES (@RequestingBuyerId, @ReceivingBuyerId, @IsApproved);";
            command.Parameters.AddWithValue("@RequestingBuyerId", requestingBuyerId);
            command.Parameters.AddWithValue("@ReceivingBuyerId", receivingBuyerId);
            command.Parameters.AddWithValue("@IsApproved", false);

            await command.ExecuteNonQueryAsync();
        }

        public async Task UpdateLinkageRequest(int requestingBuyerId, int receivingBuyerId)
        {
            await _connection.OpenConnection();
            var command = _connection.getConnection().CreateCommand();
            command.CommandText = @"UPDATE BuyerLinkage 
                                SET IsApproved=@IsApproved
                                WHERE RequestingBuyerId=@RequestingBuyerId 
                                  AND ReceivingBuyerId=@ReceivingBuyerId;";
            command.Parameters.AddWithValue("@RequestingBuyerId", requestingBuyerId);
            command.Parameters.AddWithValue("@ReceivingBuyerId", receivingBuyerId);
            command.Parameters.AddWithValue("@IsApproved", true);

            await command.ExecuteNonQueryAsync();
        }

        public async Task<bool> DeleteLinkageRequest(int requestingBuyerId, int receivingBuyerId)
        {
            await _connection.OpenConnection();
            var command = _connection.getConnection().CreateCommand();
            command.CommandText = @"DELETE FROM BuyerLinkage 
                                WHERE RequestingBuyerId=@RequestingBuyerId
                                  AND ReceivingBuyerId=@ReceivingBuyerId;";
            command.Parameters.AddWithValue("@RequestingBuyerId", requestingBuyerId);
            command.Parameters.AddWithValue("@ReceivingBuyerId", receivingBuyerId);

            return await command.ExecuteNonQueryAsync() > 0;
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
                    User = new User
                    {
                        UserId = linkedBuyerId
                    }
                },
                Status = status
            };
            return buyerLinkage;
        }


        private async Task PersistAddress(Address address, SqlConnection conn)
        {
            if (address.Id == 0)
            {
                await InsertAddress(address, conn);
            }
            else
            {
                await UpdateAddress(address, conn);
            }
        }

        private async Task UpdateAddress(Address address, SqlConnection conn)
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
            await cmd.ExecuteNonQueryAsync();
        }

        private async Task InsertAddress(Address address, SqlConnection conn)
        {
            var cmd = conn.CreateCommand();
            cmd.CommandText = @"INSERT INTO BuyerAddress(StreetLine,City,Country,PostalCode)
                            Values(@StreetLine, @City, @Country, @PostalCode); SELECT SCOPE_IDENTITY();";
            cmd.Parameters.AddWithValue("@StreetLine", address.StreetLine);
            cmd.Parameters.AddWithValue("@City", address.City);
            cmd.Parameters.AddWithValue("@Country", address.Country);
            cmd.Parameters.AddWithValue("@PostalCode", address.PostalCode);
            address.Id = Convert.ToInt32(await cmd.ExecuteScalarAsync());
        }

        public async Task<List<Buyer>> FindBuyersWithShippingAddress(Address shippingAddress)
        {
            await _connection.OpenConnection();
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
            var reader = await command.ExecuteReaderAsync();
            var buyerIds = new List<int>();
            while (await reader.ReadAsync())
            {
                buyerIds.Add(reader.GetInt32(reader.GetOrdinal("UserId")));
            }

            await reader.CloseAsync();
            return buyerIds.Select(buyerId => new Buyer
            {
                User = new User
                {
                    UserId = buyerId
                }
            }).ToList();
        }

        public async Task CreateBuyer(Buyer buyer)
        {
            await _connection.OpenConnection();
            var conn = _connection.getConnection();
            var command = conn.CreateCommand();
            await PersistAddress(buyer.BillingAddress, conn);
            if (!buyer.UseSameAddress)
            {
                await PersistAddress(buyer.ShippingAddress, conn);
            }

            command.CommandText =
                @"INSERT INTO Buyers (UserId, FirstName, LastName, BillingAddressId, ShippingAddressId, UseSameAddress, Badge,
                    TotalSpending, NumberOfPurchases, Discount)
                                VALUES (@UserID, @FirstName, @LastName,  @BillingAddressId, @ShippingAddressId, @UseSameAddress, @Badge, 0, 0,0) ";

            command.Parameters.AddWithValue("@UserId", buyer.Id);
            command.Parameters.AddWithValue("@FirstName", buyer.FirstName);
            command.Parameters.AddWithValue("@LastName", buyer.LastName);
            command.Parameters.AddWithValue("@BillingAddressId", buyer.BillingAddress.Id);
            command.Parameters.AddWithValue("@UseSameAddress", buyer.UseSameAddress);
            command.Parameters.AddWithValue("@ShippingAddressId", buyer.ShippingAddress.Id);
            command.Parameters.AddWithValue("@Badge", BuyerBadge.BRONZE.ToString());
            await command.ExecuteNonQueryAsync();
        }


        // My Market Functionalities


        // Retrieves the list of followed sellers' IDs for a specific buyer.
        public async Task<List<int>> GetFollowingUsersIds(int buyerId)
        {
            List<int> followingUsersIDs = new List<int>();

            await _connection.OpenConnection();

            var command = _connection.getConnection().CreateCommand();
            command.CommandText = "SELECT FollowedID FROM Following WHERE FollowerID = @FollowerID";
            command.Parameters.AddWithValue("@FollowerID", buyerId);

            using (var reader = await command.ExecuteReaderAsync())
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
        public async Task<List<Seller>> GetFollowedSellers(List<int>? followingUsersIds)
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
                                  $"ON u.UserId = s.UserId " +
                                  $"WHERE s.UserId IN ({formattedSellersIds})";


            List<Seller> followedSellers = new List<Seller>();
            using (var reader = await command.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    var sellerEmail = reader.GetString(0);
                    var sellerPhoneNumber = reader.GetString(1);
                    var sellerId = reader.GetInt32(2);
                    var username = reader.GetString(3);
                    var storeName = reader.GetString(4);
                    var storeDescription = reader.GetString(5);
                    var storeAddress = reader.GetString(6);
                    var followersCount = reader.GetInt32(7);
                    var trustScore = reader.GetDouble(8);

                    Seller seller = new Seller();

                    var user = new User(userID: sellerId, username: username, email: sellerEmail, phoneNumber: sellerPhoneNumber);
                    seller.User = user;

                    seller.StoreName = storeName;
                    seller.StoreDescription = storeDescription;
                    seller.StoreAddress = storeAddress;
                    seller.FollowersCount = followersCount;
                    seller.TrustScore = trustScore;

                    followedSellers.Add(seller);
                }
            }

            _connection.CloseConnection();
            return followedSellers;
        }

        // Retrieves the list of followed sellers based on a list of followed user IDs.
        public async Task<List<Seller>> GetAllSellers()
        {
            await _connection.OpenConnection();

            var command = _connection.getConnection().CreateCommand();


            // Add the followed seller IDs to the parameter (using the list of IDs)
            command.CommandText = $"SELECT u.Email, u.PhoneNumber, s.* " +
                                  $"FROM Users u " +
                                  $"INNER JOIN Sellers s " +
                                  $"ON u.UserId = s.UserId ";


            List<Seller> allSellers = new List<Seller>();
            using (var reader = await command.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    var sellerEmail = reader.GetString(0);
                    var sellerPhoneNumber = reader.GetString(1);
                    var sellerId = reader.GetInt32(2);
                    var username = reader.GetString(3);
                    var storeName = reader.GetString(4);
                    var storeDescription = reader.GetString(5);
                    var storeAddress = reader.GetString(6);
                    var followersCount = reader.GetInt32(7);
                    var trustScore = reader.GetDouble(8);

                    Seller seller = new Seller();
                    
                    var user = new User(userID: sellerId, username: username, email: sellerEmail, phoneNumber: sellerPhoneNumber);
                    seller.User = user;

                    seller.StoreName = storeName;
                    seller.StoreDescription = storeDescription;
                    seller.StoreAddress = storeAddress;
                    seller.FollowersCount = followersCount;
                    seller.TrustScore = trustScore;

                    allSellers.Add(seller);
                }
            }

            _connection.CloseConnection();
            return allSellers;
        }

        // Retrieves the list of products from a specific seller.
        public async Task<List<Product>> GetProductsFromSeller(int sellerId)
        {
            await _connection.OpenConnection();

            var command = _connection.getConnection().CreateCommand();
            command.CommandText = "SELECT * FROM Products WHERE SellerID = @SellerID";
            command.Parameters.AddWithValue("@SellerID", sellerId);

            List<Product> products = new List<Product>();

            using (var reader = await command.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    var productId = reader.GetInt32(0);
                    var productName = reader.GetString(2);
                    var productDescription = reader.GetString(3);
                    var productPrice = reader.GetDouble(4);

                    products.Add(new Product(productId, productName, productDescription, productPrice, 0, sellerId));
                }
            }

            _connection.CloseConnection();
            return products;
        }

        // Check if the Buyer has introduced his data
        public async Task<bool> CheckIfBuyerExists(int buyerId)
        {
            await _connection.OpenConnection();
            var command = _connection.getConnection().CreateCommand();
            command.CommandText = "SELECT COUNT(*) FROM Buyers WHERE UserId = @UserId";
            command.Parameters.AddWithValue("@UserId", buyerId);

            var result = await command.ExecuteScalarAsync();
            _connection.CloseConnection();

            // Return true if a record exists (i.e., count > 0)
            return Convert.ToInt32(result) > 0;
        }


        // Check if the Buyer is Following the Seller
        public async Task<bool> IsFollowing(int buyerId, int sellerId)
        {
            await _connection.OpenConnection();
            var command = _connection.getConnection().CreateCommand();
            command.CommandText =
                "SELECT COUNT(*) FROM Following WHERE FollowerID = @FollowerID AND FollowedID = @FollowedID";
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
            command.CommandText = "UPDATE Sellers SET FollowersCount = FollowersCount + 1 WHERE UserId = @UserId";
            command.Parameters.AddWithValue("@UserId", sellerId);
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
            command.CommandText = "UPDATE Sellers SET FollowersCount = FollowersCount - 1 WHERE UserId = @UserId";
            command.Parameters.AddWithValue("@UserId", sellerId);
            await command.ExecuteNonQueryAsync();

            _connection.CloseConnection();
        }

        public async Task<int> GetTotalCount()
        {
            await _connection.OpenConnection();
            var command = _connection.getConnection().CreateCommand();

            command.CommandText = "SELECT Count(*) FROM Buyers";

            var result = (int)command.ExecuteScalar();

            _connection.CloseConnection();
            return result;
        }


        public async Task UpdateAfterPurchase(Buyer buyer)
        {
            await _connection.OpenConnection();
            var conn = _connection.getConnection();
            var cmd = conn.CreateCommand();
            cmd.CommandText = @"UPDATE Buyers
                            SET 
                                TotalSpending=@TotalSpending,
                                NumberOfPurchases=@NumberOfPurchases,
                                Badge=@Badge,
                                Discount=@Discount
                            WHERE
                                UserId=@UserId;";
            cmd.Parameters.AddWithValue("@TotalSpending", buyer.TotalSpending);
            cmd.Parameters.AddWithValue("@NumberOfPurchases", buyer.NumberOfPurchases);
            cmd.Parameters.AddWithValue("@Badge", buyer.Badge.ToString());
            cmd.Parameters.AddWithValue("@Discount", buyer.Discount);
            cmd.Parameters.AddWithValue("@UserId", buyer.Id);
            await cmd.ExecuteNonQueryAsync();
        }

        public async Task RemoveWishilistItem(int buyerId, int productId)
        {
            await _connection.OpenConnection();
            var conn = _connection.getConnection();
            var cmd = conn.CreateCommand();
            cmd.CommandText = @"DELETE FROM BuyerWishlistItems
                              WHERE
                                BuyerId=@BuyerId and ProductId=@ProductId";
            cmd.Parameters.AddWithValue("@BuyerId", buyerId);
            cmd.Parameters.AddWithValue("@ProductId", productId);
            await cmd.ExecuteNonQueryAsync();
        }
    }
}