using MarketPlace924.Service;
using MarketPlace924.View;

namespace MarketPlace924.ViewModel;

public record LoginViewModel(
    UserService UserService,
    OnLoginSuccessCallback LoginSuccessCallback)
{
}