using System.ComponentModel;
using System.Runtime.CompilerServices;
using MarketPlace924.Domain;

namespace MarketPlace924.ViewModel;

public class BuyerBadgeViewModel: INotifyPropertyChanged
{
    public Buyer Buyer { get; set; } = null!;

    public int Progress
    {
        get
        {
            if (Buyer.BadgeProgress >= 95m)
            {
                return 24;
            }

            return Buyer.BadgeProgress % 25;
        }
    }

    public string Discount => "Discount " + Buyer.Discount;

    public string BadgeName => Buyer.Badge.ToString().ToLower();

    public string ImageSource => "ms-appx:///Assets/BuyerIcons/badge-" + Buyer.Badge.ToString().ToLower() + ".svg";

    public void Updated()
    {
        OnPropertyChanged(nameof(Progress));
        OnPropertyChanged(nameof(Discount));
        OnPropertyChanged(nameof(BadgeName));
        OnPropertyChanged(nameof(ImageSource));
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

}