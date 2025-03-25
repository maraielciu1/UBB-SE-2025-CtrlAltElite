using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MarketPlace924.Domain
{
    public class Buyer
    {
        // The user associated with this buyer.
        public User User { get; set; }

        // The unique identifier of the buyer, derived from the User ID.
        public int Id => User.UserId;

        // List of user IDs that this buyer is following.
        public List<int> FollowingUsersIds { get; set; }

        public Buyer()
        {
            FollowingUsersIds = new List<int>();
        }
    }
}
