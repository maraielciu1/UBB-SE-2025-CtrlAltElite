using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketPlace924.Domain
{
    public class Seller
    {
        public int SellerID { get; set; }
        public int FollowersCount { get; set; }
        public string StoreName { get; set; }
        public string StoreDescription { get; set; }
        public string StoreAddress { get; set; }
        public double TrustScore { get; set; }


        public Seller(int sellerID, string storeName, string storeDescription, string storeAddress, int followersCount = 0, double trustScore = 0)
        {
            SellerID = sellerID;
            StoreName = storeName;
            StoreDescription = storeDescription;
            StoreAddress = storeAddress;
            FollowersCount = followersCount;
            TrustScore = trustScore;
        }
    }
}
