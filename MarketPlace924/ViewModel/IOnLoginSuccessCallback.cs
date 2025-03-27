using System.Threading.Tasks;
using MarketPlace924.Domain;

namespace MarketPlace924.ViewModel;

public interface IOnLoginSuccessCallback
{
    public Task OnLoginSuccess(User user);
}