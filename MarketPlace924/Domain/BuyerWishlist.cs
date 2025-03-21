using System.Collections.Generic;

namespace MarketPlace924.Domain;

public class BuyerWishlist
{
    public List<BuyerWishlistItem> Items { get; }
    private string Code { get; }

    public BuyerWishlist()
    {
        Items = new List<BuyerWishlistItem>();
        Code = string.Empty;
    }
    
    
}