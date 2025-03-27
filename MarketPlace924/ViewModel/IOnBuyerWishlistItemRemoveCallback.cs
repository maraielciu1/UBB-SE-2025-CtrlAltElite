using System.Threading.Tasks;

namespace MarketPlace924.ViewModel;

public interface IOnBuyerWishlistItemRemoveCallback
{
    public Task OnBuyerWishlistItemRemove(int productId);
}