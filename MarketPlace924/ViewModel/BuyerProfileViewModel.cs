using System;
using System.ComponentModel;
using System.Threading.Tasks;
using MarketPlace924.Domain;
using MarketPlace924.Service;
using Microsoft.UI.Xaml.Controls;

namespace MarketPlace924.ViewModel;

public class BuyerProfileViewModel : INotifyPropertyChanged, IOnBuyerLinkageUpdatedCallback
{
    public BuyerService BuyerService { get; set; } = null!;
    public BuyerWishlistItemDetailsProvider WishlistItemDetailsProvider { get; set; } = null!;
    public User User { get; set; } = null!;

    public Buyer? Buyer { get; private set; }

    public BuyerWishlistViewModel? Wishlist { get; set; }

    public BuyerFamilySyncViewModel? FamilySync { get; set; }

    public BuyerAddressViewModel? BillingAddress { get; set; }
    public BuyerAddressViewModel? ShippingAddress { get; set; }
    public BuyerBadgeViewModel? BuyerBadge { get; set; }

    public bool CreationMode { get; set; }


    private Address? _previousAddress;

    public bool ShippingAddressEnabled => !ShippingAddressDisabled;

    public bool ShippingAddressDisabled
    {
        get => (Buyer?.UseSameAddress ?? true);
        set
        {
            if (value)
            {
                _previousAddress = Buyer!.ShippingAddress;
                Buyer.ShippingAddress = Buyer.BillingAddress;
            }
            else
            {
                Buyer!.ShippingAddress = _previousAddress ?? new Address();
            }

            ShippingAddress = new BuyerAddressViewModel(Buyer.ShippingAddress);
            Buyer.UseSameAddress = value;
            OnPropertyChanged(nameof(ShippingAddressEnabled));
            OnPropertyChanged(nameof(ShippingAddressDisabled));
            OnPropertyChanged(nameof(ShippingAddress));
        }
    }


    public async void SaveInfo()
    {
        try
        {
            if (CreationMode)
            {
                await BuyerService.CreateBuyer(Buyer!);
            }
            else
            {
                await BuyerService.SaveInfo(Buyer!);
            }

            await LoadBuyerProfile();
        }
        catch (Exception ex)
        {
            await ShowDialog("Error", ex.Message);
        }
    }

    private async Task ShowDialog(string title, string message)
    {
        var dialog = new ContentDialog
        {
            Title = title,
            Content = message,
            CloseButtonText = "OK",
            XamlRoot = App.m_window?.Content.XamlRoot
        };

        await dialog.ShowAsync();
    }


    public async void ResetInfo()
    {
        await LoadBuyerProfile();
    }


    public async Task OnBuyerLinkageUpdated()
    {
        await BuyerService.LoadBuyer(Buyer!, BuyerDataSegments.Linkages);
        Wishlist = Wishlist?.Copy();
        if (FamilySync != null)
        {
            await FamilySync.LoadLinkages();
        }
        
        OnPropertyChanged(nameof(Wishlist));
        OnPropertyChanged(nameof(FamilySync));
    }

    public void AfterPurchase()
    {
        BuyerBadge?.Updated();
        OnPropertyChanged(nameof(BuyerBadge));
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public async Task LoadBuyerProfile()
    {
        Buyer = await BuyerService.GetBuyerByUser(User);

        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        CreationMode = Buyer.FirstName == null;

        if (CreationMode)
        {
            Buyer.BillingAddress = new Address();
            Buyer.ShippingAddress = Buyer.BillingAddress;
            Buyer.UseSameAddress = true;
        }

        OnPropertyChanged(nameof(CreationMode));
        BillingAddress = new BuyerAddressViewModel(Buyer.BillingAddress);
        ShippingAddress = new BuyerAddressViewModel(Buyer.ShippingAddress);
        FamilySync = new BuyerFamilySyncViewModel(BuyerService, Buyer, this);
        await FamilySync.LoadLinkages();
        Wishlist = new BuyerWishlistViewModel
        {
            BuyerService = BuyerService,
            Buyer = Buyer,
            ItemDetailsProvider = WishlistItemDetailsProvider
        };
        BuyerBadge = new BuyerBadgeViewModel { Buyer = Buyer };
        OnPropertyChanged(nameof(Buyer));
        OnPropertyChanged(nameof(BillingAddress));
        OnPropertyChanged(nameof(ShippingAddress));
        OnPropertyChanged(nameof(ShippingAddressDisabled));
        OnPropertyChanged(nameof(ShippingAddressEnabled));
        OnPropertyChanged(nameof(Wishlist));
        OnPropertyChanged(nameof(FamilySync));
        OnPropertyChanged(nameof(BuyerBadge));
    }
}