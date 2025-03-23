using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using MarketPlace924.Domain;
using MarketPlace924.Service;
using MarketPlace924.View;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml;

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

        public string DisplayName { get; set; }
        public string Username { get; set; }
        public string FollowersCount { get; set; }
        public string StoreName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public double TrustScore { get; set; }
        public string Description { get; set; }
        public ObservableCollection<Product> Products { get; set; } = new ObservableCollection<Product>();
        public ICommand UpdateProfileCommand { get; set; }

        private void LoadSellerProfile()
        {
            var currentSeller = _sellerService.GetSeller(_username);
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

        private void LoadSellerProducts()
        {
            var currentSeller = _sellerService.GetSeller(_username);
            if (currentSeller != null)
            {
                var products = _sellerService.GetAllProducts(_sellerService.GetSellerIDByUsername(_username));
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

        private void UpdateProfile()
        {
            var currentSeller = _sellerService.GetSeller(_username);
            if (currentSeller != null)
            {
                currentSeller.StoreName = StoreName;
                currentSeller.Email = Email;
                currentSeller.PhoneNumber = PhoneNumber;
                currentSeller.StoreAddress = Address;
                currentSeller.StoreDescription = Description;
                //_sellerService.UpdateSeller(currentSeller);

                // Update the seller information in the database
                var sellerID = _sellerService.GetSellerIDByUsername(_username);
                if (sellerID > 0)
                {
                    _sellerService.UpdateSeller(currentSeller);
                }
                else
                {
                    // Handle the case where sellerID is not found
                    System.Diagnostics.Debug.WriteLine("Seller ID not found. Cannot update seller information in the database.");
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
