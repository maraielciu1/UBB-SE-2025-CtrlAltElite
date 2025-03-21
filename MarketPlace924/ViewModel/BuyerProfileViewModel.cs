using System.ComponentModel;
using MarketPlace924.Domain;
using MarketPlace924.Service;

namespace MarketPlace924.ViewModel;

public class BuyerProfileViewModel : INotifyPropertyChanged, OnBuyerLinkageUpdatedCallback
{
    private BuyerService _buyerService;
    private BuyerWishlistItemDetailsProvider _wishlistItemDetailsProvider;
    private User _user;
    private Buyer _buyer;

    public Buyer Buyer => _buyer;

    private BuyerWishlistViewModel? _wishlist;

    private BuyerFamilySyncViewModel? _familySync;

    private BuyerAddressViewModel? _billingAddress;
    private BuyerAddressViewModel? _shippingAddress;
    private Address? _previousAddress;


    public bool ShippingAddressEnabled => !_buyer.UseSameAddress;

    public bool ShippingAddressDisabled
    {
        get => _buyer.UseSameAddress;
        set
        {
            if (value)
            {
                _previousAddress = _buyer.ShippingAddress;
                _buyer.ShippingAddress = _buyer.BillingAddress;
            }
            else
            {
                _buyer.ShippingAddress = _previousAddress ?? new Address();
            }

            _shippingAddress = new BuyerAddressViewModel(_buyer.ShippingAddress);
            _buyer.UseSameAddress = value;
            OnPropertyChanged(nameof(ShippingAddressEnabled));
            OnPropertyChanged(nameof(ShippingAddressDisabled));
            OnPropertyChanged(nameof(ShippingAddress));
        }
    }


    public BuyerAddressViewModel BillingAddress
    {
        get
        {
            if (_billingAddress == null)
            {
                _billingAddress = new BuyerAddressViewModel(_buyer.BillingAddress);
            }

            return _billingAddress;
        }
    }

    public BuyerAddressViewModel ShippingAddress
    {
        get
        {
            if (_shippingAddress == null)
            {
                _shippingAddress = new BuyerAddressViewModel(_buyer.ShippingAddress);
            }

            return _shippingAddress;
        }
    }

    public BuyerWishlistViewModel Wishlist
    {
        get
        {
            if (_wishlist == null)
            {
                _wishlist = new BuyerWishlistViewModel(_buyer, _wishlistItemDetailsProvider);
            }

            return _wishlist;
        }
    }

    public BuyerFamilySyncViewModel FamilySync
    {
        get
        {
            if (_familySync == null)
            {
                _familySync = new BuyerFamilySyncViewModel(_buyerService, _buyer, this);
            }

            return _familySync;
        }
    }

    public void SaveInfo()
    {
        _buyerService.SaveInfo(_buyer);
        LoadBuyerProfile();
    }

    public void ResetInfo()
    {
        LoadBuyerProfile();
    }

    public BuyerProfileViewModel(BuyerService buyerService, User user,
        BuyerWishlistItemDetailsProvider wishlistItemDetailsProvider)
    {
        _wishlistItemDetailsProvider = wishlistItemDetailsProvider;
        _buyerService = buyerService;
        _user = user;
        _buyer = LoadBuyerProfile();
    }

    private Buyer LoadBuyerProfile()
    {
        _buyer = _buyerService.GetBuyerByUser(_user);
        _billingAddress = new BuyerAddressViewModel(_buyer.BillingAddress);
        _shippingAddress = new BuyerAddressViewModel(_buyer.ShippingAddress);
        OnPropertyChanged(nameof(Buyer));
        OnPropertyChanged(nameof(BillingAddress));
        OnPropertyChanged(nameof(ShippingAddress));
        OnPropertyChanged(nameof(ShippingAddressDisabled));
        OnPropertyChanged(nameof(ShippingAddressEnabled));
        return _buyer;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public void OnBuyerLinkageUpdated()
    {
        _buyerService.LoadBuyer(_buyer, BuyerDataSegments.Linkages);
        _wishlist = new BuyerWishlistViewModel(_wishlist!);
        OnPropertyChanged(nameof(Wishlist));
    }
}