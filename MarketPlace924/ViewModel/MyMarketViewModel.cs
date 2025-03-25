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
        // Fields
        private readonly BuyerService _buyerService;
        private readonly User _user;
        private Buyer _buyer;

        private ObservableCollection<Product> _allProducts = new ObservableCollection<Product>();
        private ObservableCollection<Product> _filteredProducts = new ObservableCollection<Product>();
        private ObservableCollection<Seller> _allFollowedSellers = new ObservableCollection<Seller>();
        private ObservableCollection<Seller> _filteredFollowedSellers = new ObservableCollection<Seller>();

        private bool _isFollowingListVisible;


        // Properties

        // Gets the buyer associated with the user.
        public Buyer Buyer => _buyer;

        // Collection of filtered products displayed in the "My Market" feed.
        public ObservableCollection<Product> MyMarketProducts
        {
            get => _filteredProducts;
            set
            {
                _filteredProducts = value;
                OnPropertyChanged(nameof(MyMarketProducts));
            }
        }

        // Collection of filtered sellers followed by the buyer.
        public ObservableCollection<Seller> MyMarketFollowing
        {
            get => _filteredFollowedSellers;
            set
            {
                _filteredFollowedSellers = value;
                OnPropertyChanged(nameof(MyMarketFollowing));
            }
        }

        // Indicates whether the following list is visible.
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

        // Determines the visibility of the following list.
        public Visibility FollowingListVisibility => IsFollowingListVisible ? Visibility.Visible : Visibility.Collapsed;

        // Determines the visibility of the button to show the following list.
        public Visibility ShowFollowingVisibility => IsFollowingListVisible ? Visibility.Collapsed : Visibility.Visible;



        // Commands

        // Command to toggle the visibility of the following list.
        public ICommand ShowFollowingCommand { get; }



        // Constructor
        public MyMarketViewModel(BuyerService buyerService, User user)
        {
            _buyerService = buyerService;
            _user = user;

            ShowFollowingCommand = new RelayCommand(ShowFollowingList);
            IsFollowingListVisible = false;

            LoadData();
        }


        // Methods

        // Loads the buyer information and market data.
        private async void LoadData()
        {
            _buyer = await _buyerService.GetBuyerByUser(_user);
            OnPropertyChanged(nameof(Buyer));

            await LoadMyMarketData();
        }

        // Toggles the visibility of the following list.
        private async void ShowFollowingList(object parameter)
        {
            IsFollowingListVisible = !IsFollowingListVisible;

            if (IsFollowingListVisible)
            {
                await LoadFollowing();
            }
        }

        // Filters products based on the search query.
        public void FilterProducts(string searchText)
        {
            _filteredProducts.Clear();

            if (string.IsNullOrEmpty(searchText))
            {
                foreach (var product in _allProducts)
                {
                    _filteredProducts.Add(product);
                }
            }
            else
            {
                var filteredProducts = _allProducts.Where(p => p.Name.Contains(searchText, StringComparison.OrdinalIgnoreCase)).ToList();
                foreach (var product in filteredProducts)
                {
                    _filteredProducts.Add(product);
                }
            }

            OnPropertyChanged(nameof(MyMarketProducts));
        }

        // Filters the followed sellers based on the search query.
        public void FilterFollowing(string searchText)
        {
            _filteredFollowedSellers.Clear();

            if (string.IsNullOrEmpty(searchText))
            {
                foreach (var seller in _allFollowedSellers)
                {
                    _filteredFollowedSellers.Add(seller);
                }
            }
            else
            {
                var filteredSellers = _allFollowedSellers.Where(p => p.StoreName.Contains(searchText, StringComparison.OrdinalIgnoreCase)).ToList();
                foreach (var seller in filteredSellers)
                {
                    _filteredFollowedSellers.Add(seller);
                }
            }

            OnPropertyChanged(nameof(MyMarketFollowing));
        }

        // Loads the products from the followed sellers.
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

        // Loads the followed sellers.
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

                FilterFollowing(""); // Show all followed sellers initially
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading followed sellers: {ex.Message}");
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
