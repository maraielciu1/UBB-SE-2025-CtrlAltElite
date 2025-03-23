using System;

namespace MarketPlace924.Domain
{
    public class Seller : User
    {
        public int FollowersCount { get; set; }
        public string StoreName { get; set; }
        public string StoreDescription { get; set; }
        public  string StoreAddress { get; set; }
        public double TrustScore { get; set; }


        public Seller(User user, string storeName, string storeDescription, string storeAddress, int followersCount = 0, double trustScore = 0)
            : base(user.UserId, user.Username, user.Email, user.PhoneNumber, user.Password, user.Role, user.FailedLogins, user.BannedUntil, user.IsBanned)
        {
            StoreName = storeName;
            StoreDescription = storeDescription;
            StoreAddress = storeAddress;
            FollowersCount = followersCount;
            TrustScore = trustScore;
        }

        //User user = new User(1, "username", "email@example.com", "1234567890", "password", UserRole.Seller);
        //Address address = new Address { ID = 1, StreetLine = "123 Main St", City = "Anytown", Country = "USA", PostalCode = "12345" };

        //Seller seller = new Seller(user, "My Store", "Best store in town", address, 100);

    }
}
