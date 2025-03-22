using System;
using System.Collections.Generic;

namespace MarketPlace924.Domain
{
    public class Buyer
    {
        public User User { get; set; }

        public string PhoneNumber
        {
            get => User.PhoneNumber;
            set => User.PhoneNumber = value;
        }

        public string Email
        {
            get => User.Email;
            set => User.Email = value;
        }

        public int ID
        {
            get => User.UserID;
        }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool UseSameAddress { get; set; }
        public BuyerBadge Badge { get; set; }
        public decimal TotalSpending { get; set; }
        public decimal Discount { get; set; }
        public Address ShippingAddress { get; set; }
        public Address BillingAddress { get; set; }
        public List<Buyer> SyncedBuyerIds { get; set; }
        public List<Wishlist> Wishlist { get; set; }

        public Buyer()
        {
            Wishlist = new List<Wishlist>();
            Wishlist.Add(new Wishlist());
        }
    }
}