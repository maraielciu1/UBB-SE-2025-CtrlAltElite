using System;
using System.Collections.Generic;
using System.Linq;
using MarketPlace924.Domain;
using Microsoft.Data.SqlClient;

namespace MarketPlace924.Repository;

public class BuyerRepository
{
    private DBConnection.DatabaseConnection _connection;

    public BuyerRepository(DBConnection.DatabaseConnection connection)
    {
        _connection = connection;
    }

    public void LoadBuyerInfo(Buyer buyer)
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
                User = new User
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
        command.Parameters.AddWithValue("@StreetLine",shippingAddress.StreetLine);
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
            User = new User
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
}