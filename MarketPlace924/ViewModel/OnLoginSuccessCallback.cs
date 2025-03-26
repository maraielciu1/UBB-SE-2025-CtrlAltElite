using MarketPlace924.Domain;

namespace MarketPlace924.View;

public interface OnLoginSuccessCallback
{
    public void OnLoginSuccess(User user);
}