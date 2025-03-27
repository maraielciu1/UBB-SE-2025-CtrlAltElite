using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using MarketPlace924.Domain;
using MarketPlace924.Service;

namespace MarketPlace924.ViewModel;

public class BuyerWishlistViewModel : INotifyPropertyChanged, IOnBuyerWishlistItemRemoveCallback
{
    private List<BuyerWishlistItemViewModel>? _allItems;
    private string _searchText = string.Empty;
    private ObservableCollection<BuyerWishlistItemViewModel>? _items;
    public Buyer Buyer { get; set; } = null!;
    public BuyerWishlistItemDetailsProvider ItemDetailsProvider { get; set; } = null!;
    private bool _familySyncActive;
    private string? _selectedSort;
    public BuyerService BuyerService { get; set; } = null!;


    public BuyerWishlistViewModel Copy()
    {
        return new BuyerWishlistViewModel
        {
            Buyer = Buyer,
            ItemDetailsProvider = ItemDetailsProvider,
            BuyerService = BuyerService,
            _searchText = _searchText,
            _familySyncActive = _familySyncActive,
            _selectedSort = _selectedSort
        };
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


    public string? SelectedSort
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

    public async Task OnBuyerWishlistItemRemove(int productId)
    {
        await BuyerService.RemoveWishilistItem(Buyer, productId);
        _items = null;
        OnPropertyChanged(nameof(Items));
    }

    private List<BuyerWishlistItemViewModel> ComputeAllItems()
    {
        var ownItems = Buyer.Wishlist.Items.Select(x => GetWishlistItemDetails(x, true));
        var linkedItems = Buyer.Linkages.Where(link => link.Status == BuyerLinkageStatus.Confirmed)
            .Select(link => link.Buyer.Wishlist.Items).SelectMany(list => list)
            .Select(wishlistItem => GetWishlistItemDetails(wishlistItem));
        return ownItems.Concat(linkedItems).GroupBy(x => x.ProductId)
            .Select(itemsWithSameProduct => itemsWithSameProduct
                .OrderByDescending(item => item.OwnItem).First()).ToList();
    }

    private BuyerWishlistItemViewModel GetWishlistItemDetails(BuyerWishlistItem wishlistItem, bool canDelete = false)
    {
        var item = ItemDetailsProvider.LoadWishlistItemDetails(new BuyerWishlistItemViewModel
        {
            ProductId = wishlistItem.ProductId,
            OwnItem = canDelete,
            RemoveCallback = this
        });
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