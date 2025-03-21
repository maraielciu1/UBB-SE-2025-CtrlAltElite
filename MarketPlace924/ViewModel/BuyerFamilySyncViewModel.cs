using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using MarketPlace924.Domain;
using MarketPlace924.Service;

namespace MarketPlace924.ViewModel;

public class BuyerFamilySyncViewModel : INotifyPropertyChanged
{
    private List<BuyerLinkageViewModel>? _allItems;
    private ObservableCollection<BuyerLinkageViewModel>? _items;
    private Buyer _currentBuyer;
    private BuyerService _service;

    private OnBuyerLinkageUpdatedCallback _linkageUpdatedCallback;

    public BuyerFamilySyncViewModel(BuyerService service, Buyer buyer,
        OnBuyerLinkageUpdatedCallback linkageUpdatedCallback)
    {
        _service = service;
        _currentBuyer = buyer;
        _linkageUpdatedCallback = linkageUpdatedCallback;
    }

    public ObservableCollection<BuyerLinkageViewModel> Items
    {
        get
        {
            if (_items == null)
            {
                _allItems = LoadLinkages();
                _items = new ObservableCollection<BuyerLinkageViewModel>(_allItems);
            }

            return _items;
        }
    }

    private List<BuyerLinkageViewModel> LoadLinkages()
    {
        var household = _service.FindBuyersWithShippingAddress(_currentBuyer.ShippingAddress)
            .Where(householdBuyer => householdBuyer.Id != _currentBuyer.Id).ToList();
        var linkages = _currentBuyer.Linkages;
        var availableLinkages = household.Select(buyer => NewBuyerLinkageViewModel(buyer, BuyerLinkageStatus.Possible));

        var existingLinkages = linkages.Select(linkage => NewBuyerLinkageViewModel(linkage.Buyer, linkage.Status));
        return availableLinkages.Concat(existingLinkages)
            .GroupBy(link => link.LinkedBuyer.Id)
            .Select(group => group.OrderByDescending(linkage => linkage.Status).First())
            .ToList();
    }

    private BuyerLinkageViewModel NewBuyerLinkageViewModel(Buyer buyer, BuyerLinkageStatus status)
    {
        return new BuyerLinkageViewModel
        {
            LinkageUpdatedCallback = _linkageUpdatedCallback,
            Service = _service,
            UserBuyer = _currentBuyer,
            LinkedBuyer = buyer,
            Status = status
        };
    }


    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}