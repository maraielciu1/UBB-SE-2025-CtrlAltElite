using MarketPlace924.Domain;
using MarketPlace924.Service;
using Microsoft.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MarketPlace924.ViewModel
{
    public class MyMarketViewModel : INotifyPropertyChanged
    {
        // Services and User Data
        private readonly BuyerService _buyerService;
        private readonly User _user;
        private Buyer _buyer;

        // Collections for Products and Followed Sellers
        private ObservableCollection<Product> _allProducts = new ObservableCollection<Product>();
        private ObservableCollection<Product> _filteredProducts = new ObservableCollection<Product>();
        private ObservableCollection<Seller> _allFollowedSellers = new ObservableCollection<Seller>();
        private ObservableCollection<Seller> _filteredFollowedSellers = new ObservableCollection<Seller>();

        // UI State Management
        private bool _isFollowingListVisible;
        private int _followedSellersCount;

        // Properties

        // Buyer service accessor
        public BuyerService BuyerService { get; }

        // Buyer information
        public Buyer Buyer => _buyer;

        // Filtered products displayed in "My Market" feed
        public ObservableCollection<Product> MyMarketProducts
        {
            get => _filteredProducts;
            set
            {
                _filteredProducts = value;
                OnPropertyChanged(nameof(MyMarketProducts));
            }
        }

        // Filtered followed sellers list
        public ObservableCollection<Seller> MyMarketFollowing
        {
            get => _filteredFollowedSellers;
            set
            {
                _filteredFollowedSellers = value;
                OnPropertyChanged(nameof(MyMarketFollowing));
            }
        }

        // Number of followed sellers
        public int FollowedSellersCount
        {
            get => _followedSellersCount;
            private set
            {
                _followedSellersCount = value;
                OnPropertyChanged(nameof(FollowedSellersCount));
            }
        }

        // Indicates if the following list is visible
        public bool IsFollowingListVisible
        {
            get => _isFollowingListVisible;
            set
            {
                if (_isFollowingListVisible != value)
                {
                    _isFollowingListVisible = value;
                    OnPropertyChanged(nameof(IsFollowingListVisible));
                    OnPropertyChanged(nameof(FollowingListVisibility));
                    OnPropertyChanged(nameof(ShowFollowingVisibility));
                }
            }
        }

        // Visibility for the following list
        public Visibility FollowingListVisibility => IsFollowingListVisible ? Visibility.Visible : Visibility.Collapsed;

        // Visibility for the button to show the following list
        public Visibility ShowFollowingVisibility => IsFollowingListVisible ? Visibility.Collapsed : Visibility.Visible;

        // Command to toggle following list visibility
        public ICommand ShowFollowingCommand { get; }


        public MyMarketViewModel(BuyerService buyerService, User user)
        {
            _buyerService = buyerService;
            _user = user;

            BuyerService = _buyerService;
            ShowFollowingCommand = new RelayCommand(ShowFollowingList);
            IsFollowingListVisible = false;

            LoadData();
        }

        // Loads buyer and market data
        private async Task LoadData()
        {
            _buyer = await _buyerService.GetBuyerByUser(_user);
            OnPropertyChanged(nameof(Buyer));

            await LoadFollowing();
            await LoadMyMarketData();
        }

        // Toggles following list visibility and reloads data if shown
        private async void ShowFollowingList(object parameter)
        {
            IsFollowingListVisible = !IsFollowingListVisible;
            if (IsFollowingListVisible)
            {
                await LoadFollowing();
            }
        }

        // Filters products based on search query
        public void FilterProducts(string searchText)
        {
            _filteredProducts.Clear();
            var filteredProducts = string.IsNullOrEmpty(searchText)
                ? _allProducts
                : _allProducts.Where(p => p.Name.Contains(searchText, StringComparison.OrdinalIgnoreCase));

            foreach (var product in filteredProducts)
            {
                _filteredProducts.Add(product);
            }
            OnPropertyChanged(nameof(MyMarketProducts));
        }

        // Filters followed sellers based on search query
        public void FilterFollowing(string searchText)
        {
            _filteredFollowedSellers.Clear();
            var filteredSellers = string.IsNullOrEmpty(searchText)
                ? _allFollowedSellers
                : _allFollowedSellers.Where(s => s.StoreName.Contains(searchText, StringComparison.OrdinalIgnoreCase));

            foreach (var seller in filteredSellers)
            {
                _filteredFollowedSellers.Add(seller);
            }
            OnPropertyChanged(nameof(MyMarketFollowing));
        }

        // Loads products from followed sellers
        public async Task LoadMyMarketData()
        {
            try
            {
                var products = await _buyerService.GetProductsFromFollowedSellers(_buyer.FollowingUsersIds);
                _allProducts.Clear();
                foreach (var product in products)
                {
                    _allProducts.Add(product);
                }
                FilterProducts(""); // Show all products initially
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading market data: {ex.Message}");
            }
        }

        // Loads the followed sellers list
        public async Task LoadFollowing()
        {
            try
            {
                var sellers = await _buyerService.GetFollowedSellers(_buyer.FollowingUsersIds);
                _allFollowedSellers.Clear();
                foreach (var seller in sellers)
                {
                    _allFollowedSellers.Add(seller);
                }
                FollowedSellersCount = _allFollowedSellers.Count;
                FilterFollowing(""); // Show all followed sellers initially
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading followed sellers: {ex.Message}");
            }
        }

        // Refreshes buyer and market data
        public async Task RefreshData()
        {
            try
            {
                await LoadData();
                await LoadFollowing();
                await LoadMyMarketData();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error refreshing data: {ex.Message}");
            }
        }

        // PropertyChanged event for UI updates
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
