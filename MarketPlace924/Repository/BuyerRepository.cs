using System;
using System.Data;
using MarketPlace924.Domain;
using Microsoft.Data.SqlClient;

namespace MarketPlace924.Repository;

public class BuyerRepository
{
    private DBConnection.DBConnection _connection;

    public BuyerRepository(DBConnection.DBConnection connection)
    {
        _connection = connection;
    }

    public Buyer? GetBuyer(int userID)
    {
        var conn = _connection.getConnection();
        conn.Open();
        var cmd = conn.CreateCommand();
        cmd.CommandText = @"select 
                    FirstName, LastName, Badge, 
                    TotalSpending, Discount, 
                    ShippingAddressId, BillingAddressId, UseSameAddress
                from Buyers where UserID = @userID";
        cmd.Parameters.AddWithValue("@userID", userID);
        var reader = cmd.ExecuteReader();
        if (!reader.Read())
        {
            return null;
        }

        var useSameAddress = reader.GetBoolean(reader.GetOrdinal("UseSameAddress"));
        var billingAddressId = reader.GetInt32(reader.GetOrdinal("BillingAddressId"));
        var shippingAddressId = reader.GetInt32(reader.GetOrdinal("ShippingAddressId"));
        var bayer = new Buyer
        {
            User = new User
            {
                UserID = userID
            },
            FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
            LastName = reader.GetString(reader.GetOrdinal("LastName")),
            Badge = ParseBadge(reader.GetString(reader.GetOrdinal("Badge"))),
            TotalSpending = reader.GetDecimal(reader.GetOrdinal("TotalSpending")),
            Discount = reader.GetDecimal(reader.GetOrdinal("Discount")),
            UseSameAddress = useSameAddress
        };
        
        reader.Close();
        
        var billingAddress = LoadAddress(billingAddressId, conn)!;
        bayer.BillingAddress = billingAddress;
        bayer.ShippingAddress = useSameAddress
            ? billingAddress
            : LoadAddress(shippingAddressId, conn)!;
        conn.Close();
        return bayer;
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
            ID = addressId,
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
        var conn = _connection.getConnection();
        conn.Open();
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
        command.Parameters.AddWithValue("@BillingAddressId", buyer.BillingAddress.ID);
        command.Parameters.AddWithValue("@UseSameAddress", buyer.UseSameAddress);
        command.Parameters.AddWithValue("@ShippingAddressId", buyer.ShippingAddress.ID);
        command.Parameters.Add(new SqlParameter("@UserID", buyer.User.UserID));
        command.ExecuteNonQuery();
        conn.Close();
    }

    private void PersistAddress(Address address, SqlConnection conn)
    {
        if (address.ID == 0)
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
        cmd.Parameters.AddWithValue("@ID", address.ID);
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
        address.ID = Convert.ToInt32(cmd.ExecuteScalar());
    }
}