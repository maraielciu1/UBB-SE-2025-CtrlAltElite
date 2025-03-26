using System.Collections.Generic;

namespace MarketPlace924.Domain
{
    public class Buyer
    {
        // The user associated with this buyer.
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

        // The unique identifier of the buyer, derived from the User ID.
        public int Id => User.UserId;

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool UseSameAddress { get; set; }
        public BuyerBadge Badge { get; set; }
        public decimal TotalSpending { get; set; }
        public int NumberOfPurchases { get; set; }
        public decimal Discount { get; set; }
        public Address ShippingAddress { get; set; }
        public Address BillingAddress { get; set; }
        public List<Buyer> SyncedBuyerIds { get; set; }
        public BuyerWishlist Wishlist { get; set; }
        public List<BuyerLinkage> Linkages { get; set; }

        // List of user IDs that this buyer is following.
        public List<int> FollowingUsersIds { get; set; }

        public Buyer()
        {
            Wishlist = new BuyerWishlist();
            FollowingUsersIds = new List<int>();
        }
    }
}

