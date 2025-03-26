namespace MarketPlace924.Domain;

public class BuyerWishlistItem
{
    private int _productId;
    public int ProductId => _productId;

    public BuyerWishlistItem(int productId)
    {
        _productId = productId;
    }
}