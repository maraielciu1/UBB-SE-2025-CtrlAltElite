using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using MarketPlace924.Domain;

namespace MarketPlace924.ViewModel;

public class BuyerWishlistViewModel : INotifyPropertyChanged
{
    private List<BuyerWishlistItemViewModel>? _allItems;
    private string _searchText = "";
    private ObservableCollection<BuyerWishlistItemViewModel>? _items;
    private Buyer _buyer;
    private BuyerWishlistItemDetailsProvider _itemDetailsProvider;
    private bool _familySyncActive;
    private string _selectedSort;


    public BuyerWishlistViewModel(BuyerWishlistViewModel copySource)
    {
        _buyer = copySource._buyer;
        _itemDetailsProvider = copySource._itemDetailsProvider;
        _searchText = copySource._searchText;
        _familySyncActive = copySource._familySyncActive;
        _selectedSort = copySource._selectedSort;
    }

    public BuyerWishlistViewModel(Buyer buyer, BuyerWishlistItemDetailsProvider itemDetailsProvider)
    {
        _buyer = buyer;
        _itemDetailsProvider = itemDetailsProvider;
    }

    public ObservableCollection<BuyerWishlistItemViewModel> Items
    {
        get
        {
            if (_items == null)
            {
                _allItems = ComputeAllItems();
                UpdateItems();
            }

            return _items!;
        }
    }


    public string SearchText
    {
        get => _searchText;
        set
        {
            _searchText = value;
            UpdateItems();
        }
    }

    public ObservableCollection<string> SortOptions { get; } = new()
    {
        "Sort by: Price Ascending", "Sort by: Price Descending"
    };


    public string SelectedSort
    {
        get => _selectedSort;
        set
        {
            _selectedSort = value;
            UpdateItems();
        }
    }

    public bool FamilySyncActive
    {
        get => _familySyncActive;
        set
        {
            _familySyncActive = value;
            UpdateItems();
        }
    }


    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private List<BuyerWishlistItemViewModel> ComputeAllItems()
    {
        var ownItems = _buyer.Wishlist.Items.Select(x => GetWishlistItemDetails(x, true));
        var linkedItems = _buyer.Linkages.Where(link => link.Status == BuyerLinkageStatus.Confirmed)
            .Select(link => link.Buyer.Wishlist.Items).SelectMany(list => list)
            .Select(wishlistItem => GetWishlistItemDetails(wishlistItem));
        return ownItems.Concat(linkedItems).GroupBy(x => x.ProductId)
            .Select(itemsWithSameProduct => itemsWithSameProduct
                .OrderByDescending(item => item.OwnItem).First()).ToList();
    }

    private BuyerWishlistItemViewModel GetWishlistItemDetails(BuyerWishlistItem wishlistItem, bool canDelete = false)
    {
        var item = _itemDetailsProvider.GetWishlistItemDetails(wishlistItem.ProductId);
        item.OwnItem = canDelete;
        item.ProductId = wishlistItem.ProductId;
        return item;
    }

    private void UpdateItems()
    {
        var newItems = _allItems!;
        var enumerable = newItems.AsEnumerable();

        if (!_familySyncActive)
        {
            enumerable = enumerable.Where(x => x.OwnItem);
        }

        if (_searchText.Length > 0)
        {
            enumerable = enumerable
                .Where(x => x.Title.ToUpper().Contains(_searchText.ToUpper()));
        }

        if ("Sort by: Price Descending" == _selectedSort)
        {
            enumerable = enumerable.OrderByDescending(x => x.Price);
        }
        else if ("Sort by: Price Ascending" == _selectedSort)
        {
            enumerable = enumerable.OrderBy(x => x.Price);
        }

        _items = new ObservableCollection<BuyerWishlistItemViewModel>(enumerable.ToList());

        OnPropertyChanged(nameof(Items));
    }
}