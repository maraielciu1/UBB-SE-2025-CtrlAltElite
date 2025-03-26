using MarketPlace924.Domain;
using MarketPlace924.Service;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.System;
using Windows.UI.Notifications;

namespace MarketPlace924.ViewModel
{
    public class MyMarketProfileViewModel : INotifyPropertyChanged
    {
        // Private fields to store buyer, seller, and service information
        private Buyer _buyer;
        private Seller _seller;
        private BuyerService _buyerService;

        // Collections to store all products and filtered products for display
        private ObservableCollection<Product> _allProducts;
        private ObservableCollection<Product> _filteredProducts;

        // Property to track whether the buyer is following the seller
        private bool _isFollowing;
        public bool IsFollowing
        {
            get => _isFollowing;
            set
            {
                _isFollowing = value;
                OnPropertyChanged(nameof(IsFollowing));
                OnPropertyChanged(nameof(FollowButtonText));
                OnPropertyChanged(nameof(FollowButtonColor)); // Update button color when follow status changes
            }
        }

        // Text displayed on the follow/unfollow button
        public string FollowButtonText => IsFollowing ? "Unfollow" : "Follow";

        // Color of the follow/unfollow button
        public string FollowButtonColor => IsFollowing ? "Red" : "White";

        // Commands for user interactions
        public ICommand FollowCommand { get; }


        // Public property for displaying filtered products
        public ObservableCollection<Product> SellerProducts
        {
            get => _filteredProducts;
            set
            {
                _filteredProducts = value;
                OnPropertyChanged(nameof(SellerProducts));
            }
        }

        // Collection to store notifications
        private ObservableCollection<string> _notifications = new ObservableCollection<string>();
        public ObservableCollection<string> Notifications
        {
            get => _notifications;
            set
            {
                _notifications = value;
                OnPropertyChanged(nameof(Notifications));
            }
        }


        // Public properties for displaying seller profile details
        public string DisplayName { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string FollowersCount { get; set; } = string.Empty;
        public string StoreName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public double TrustScore { get; set; }
        public string Description { get; set; } = string.Empty;

        // Constructor initializing services, buyer, seller, and commands
        public MyMarketProfileViewModel(BuyerService buyerService, Buyer buyer, Seller seller)
        {
            _buyerService = buyerService;
            _buyer = buyer;
            _seller = seller;

            _allProducts = new ObservableCollection<Product>();
            SellerProducts = new ObservableCollection<Product>();

            FollowCommand = new RelayCommand(ToggleFollow);

            LoadMyMarketProfileData(); // Load initial data
        }

        // Loads the seller's profile data
        private async Task LoadMyMarketProfileData()
        {
            await LoadSellerProducts();
            await CheckFollowStatus();
            await LoadSellerProfile();
        }

        // Toggles follow/unfollow state for the seller
        private async Task ToggleFollow()
        {
            if (IsFollowing)
            {
                await _buyerService.UnfollowSeller(_buyer.Id, _seller.Id); // Unfollow seller
            }
            else
            {
                await _buyerService.FollowSeller(_buyer.Id, _seller.Id); // Follow seller
            }

            IsFollowing = !IsFollowing; // Update follow status
        }

        // Loads seller profile details and updates UI
        private async Task LoadSellerProfile()
        {
            if (_seller != null)
            {
                StoreName = _seller.StoreName;
                Username = _seller.Username;
                Email = _seller.Email;
                PhoneNumber = _seller.PhoneNumber;
                Address = _seller.StoreAddress;
                FollowersCount = _seller.FollowersCount.ToString();
                TrustScore = _seller.TrustScore * 100.0 / 5.0;
                Description = _seller.StoreDescription;

                // Notify the UI of property changes
                OnPropertyChanged(nameof(DisplayName));
                OnPropertyChanged(nameof(StoreName));
                OnPropertyChanged(nameof(Email));
                OnPropertyChanged(nameof(PhoneNumber));
                OnPropertyChanged(nameof(Address));
                OnPropertyChanged(nameof(Description));
                OnPropertyChanged(nameof(FollowersCount));
                OnPropertyChanged(nameof(TrustScore));
            }
        }

        // Loads the seller's products
        private async Task LoadSellerProducts()
        {
            if (_seller != null)
            {
                var products = await _buyerService.GetProductsForViewProfile(_seller.Id);
                if (products != null)
                {
                    _allProducts.Clear();
                    foreach (var product in products)
                    {
                        _allProducts.Add(product);
                    }
                    FilterProducts(""); // Update filtered products
                }
            }
        }

        // Filters products based on a search query
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

            OnPropertyChanged(nameof(SellerProducts));
        }

        // Checks if the buyer follows the seller
        private async Task CheckFollowStatus()
        {
            var isFollowing = await _buyerService.IsFollowing(_buyer.Id, _seller.Id);
            IsFollowing = isFollowing;
            OnPropertyChanged(nameof(IsFollowing));
        }


        // Notifies the UI of property changes
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
