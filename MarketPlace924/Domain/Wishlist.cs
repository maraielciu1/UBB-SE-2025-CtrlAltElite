using System.Collections.Generic;

namespace MarketPlace924.Domain;

public class Wishlist
{
    private List<string> Items { get; }
    private string Code { get; }

    public Wishlist()
    {
        Items = new List<string>();
        Code = string.Empty;
    }
    
    
}