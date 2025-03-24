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
    }
}
