namespace MarketPlace924.ViewModel;

public class BuyerWishlistItemViewModel
{
    public int ProductId { get; set; }
    public string Title { get; set; }
    public decimal Price { get; set; }

    public string Description { get; set; }

    public string ImageSource { get; set; }

    public bool OwnItem { get; set; }
}