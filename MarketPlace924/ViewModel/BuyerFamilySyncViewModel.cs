using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using MarketPlace924.Domain;
using MarketPlace924.Service;

namespace MarketPlace924.ViewModel;

public class BuyerFamilySyncViewModel : INotifyPropertyChanged
{
    private List<BuyerLinkageViewModel>? _allItems;
    public ObservableCollection<BuyerLinkageViewModel>? Items { get; set; } = new();
    private Buyer _currentBuyer;
    private BuyerService _service;

    private IOnBuyerLinkageUpdatedCallback _linkageUpdatedCallback;

    public BuyerFamilySyncViewModel(BuyerService service, Buyer buyer,
        IOnBuyerLinkageUpdatedCallback linkageUpdatedCallback)
    {
        _service = service;
        _currentBuyer = buyer;
        _linkageUpdatedCallback = linkageUpdatedCallback;
    }

    public async Task LoadLinkages()
    {
        
        _allItems = await LoadAllPossibleLinkages();
        Items = new ObservableCollection<BuyerLinkageViewModel>(_allItems);
        OnPropertyChanged(nameof(Items));
    }
    private async Task<List<BuyerLinkageViewModel>> LoadAllPossibleLinkages()
    {
        var household = (await _service.FindBuyersWithShippingAddress(_currentBuyer.ShippingAddress))
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

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}