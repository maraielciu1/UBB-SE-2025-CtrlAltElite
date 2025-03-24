using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using MarketPlace924.Domain;
using MarketPlace924.Service;

namespace MarketPlace924.ViewModel
{
    public class SellerProfileViewModel : INotifyPropertyChanged
    {
        private UserService _userService;
        private SellerService _sellerService;
        private string _username;

        private ObservableCollection<Product> _allProducts;
        public ObservableCollection<Product> FilteredProducts { get; set; }

        public SellerProfileViewModel(UserService userService, SellerService sellerService, string username)
        {
            _userService = userService;
            _sellerService = sellerService;
            _username = username;
            _allProducts = new ObservableCollection<Product>();
            FilteredProducts = new ObservableCollection<Product>();
            LoadSellerProfile();
            LoadSellerProducts();
            UpdateProfileCommand = new CustomCommand(UpdateProfile);
        }

        public string DisplayName { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string FollowersCount { get; set; } = string.Empty;
        public string StoreName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public double TrustScore { get; set; }
        public string Description { get; set; } = string.Empty;
        public ObservableCollection<Product> Products { get; set; } = new ObservableCollection<Product>();
        public ICommand UpdateProfileCommand { get; set; }

        private async void LoadSellerProfile()
        {
            var currentSeller = await _sellerService.GetSellerAsync(_username);
            if (currentSeller != null)
            {
                StoreName = currentSeller.StoreName;
                Username = currentSeller.Username;
                Email = currentSeller.Email;
                PhoneNumber = currentSeller.PhoneNumber;
                Address = currentSeller.StoreAddress;
                FollowersCount = currentSeller.FollowersCount.ToString();
                TrustScore = currentSeller.TrustScore;
                Description = currentSeller.StoreDescription;
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

        private async void LoadSellerProducts()
        {
            var currentSeller = await _sellerService.GetSellerAsync(_username);
            if (currentSeller != null)
            {
                var products = await _sellerService.GetAllProductsAsync(await _sellerService.GetSellerIDByUsernameAsync(_username));
                if (products != null)
                {
                    _allProducts.Clear();
                    foreach (var product in products)
                    {
                        _allProducts.Add(product);
                    }
                    FilterProducts(string.Empty); // Update FilteredProducts after loading
                }
            }
        }

        public void FilterProducts(string searchText)
        {
            if (string.IsNullOrEmpty(searchText))
            {
                FilteredProducts = new ObservableCollection<Product>(_allProducts);
            }
            else
            {
                var filtered = _allProducts.Where(p => p.Name.Contains(searchText, StringComparison.OrdinalIgnoreCase)).ToList();
                FilteredProducts = new ObservableCollection<Product>(filtered);
            }
            OnPropertyChanged(nameof(FilteredProducts));
        }

        private async void UpdateProfile()
        {
            var currentSeller = await _sellerService.GetSellerAsync(_username);
            if (currentSeller != null)
            {
                currentSeller.StoreName = StoreName;
                currentSeller.Email = Email;
                currentSeller.PhoneNumber = PhoneNumber;
                currentSeller.StoreAddress = Address;
                currentSeller.StoreDescription = Description;

                // Update the seller information in the database
                var sellerID = await _sellerService.GetSellerIDByUsernameAsync(_username);
                if (sellerID > 0)
                {
                    await _sellerService.UpdateSellerAsync(currentSeller);
                }
                else
                {
                    // Handle the case where sellerID is not found
                    System.Diagnostics.Debug.WriteLine("Seller ID not found. Cannot update seller information in the database.");
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
