using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketPlace924.Domain
{
    public class Seller
    {
        public User User { get; set; }
        public int Id => User.UserId;
        public string Email => User.Email;
        public string PhoneNumber => User.PhoneNumber;

        public string Username => User.Username;
        public int FollowersCount { get; set; }
        public string StoreName { get; set; }
        public string StoreDescription { get; set; }
        public string StoreAddress { get; set; }
        public double TrustScore { get; set; }


        public Seller()
        {
        }
    }
}