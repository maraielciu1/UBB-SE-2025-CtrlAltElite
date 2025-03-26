using System;

namespace MarketPlace924.Service;


[Flags]
public enum BuyerDataSegments
{
    BasicInfo = 1,
    User = 2,
    Wishlist = 4,
    Linkages = 8
}