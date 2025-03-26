using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using MarketPlace924.Domain;
using MarketPlace924.Service;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml;
using Windows.Networking.NetworkOperators;
using System.Threading.Tasks;

namespace MarketPlace924.ViewModel
{
    public class SellerProfileViewModel : INotifyPropertyChanged
    {
        private UserService _userService;
        private SellerService _sellerService;
        private User _user;
        private Seller _seller;

        public Seller Seller => _seller;

        private ObservableCollection<Product> _allProducts;
        public ObservableCollection<Product> FilteredProducts { get; set; }

        public SellerProfileViewModel(User user, UserService userService, SellerService sellerService)
        {
            _userService = userService;
            _sellerService = sellerService;
            _user = user;
            _allProducts = new ObservableCollection<Product>();
            FilteredProducts = new ObservableCollection<Product>();
            UpdateProfileCommand = new CustomCommand(UpdateProfile);
            LoadSellerData();
        }

        private async void LoadSellerData()
        {
            _seller = _sellerService.GetSellerByUser(_user);
            OnPropertyChanged(nameof(Seller));

            LoadSellerProfile();
            await LoadSellerProducts();
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

        private void LoadSellerProfile()
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

        private async Task LoadSellerProducts()
        {
            if (_seller != null)
            {
                var products = await _sellerService.GetAllProductsAsync(_seller.Id);
                if (products != null)
                {
                    _allProducts.Clear();
                    foreach (var product in products)
                    {
                        _allProducts.Add(product);
                    }
                    FilterProducts(string.Empty);
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
            if (_seller != null)
            {
                _seller.StoreName = StoreName;
                //currentSeller.Email = Email;
                //currentSeller.PhoneNumber = PhoneNumber;
                _seller.StoreAddress = Address;
                _seller.StoreDescription = Description;

                if (_seller.Id > 0)
                {
                    await _sellerService.UpdateSellerAsync(_seller);
                    // Navigate to the desired page after successful update
                    //NavigateToProfilePage();
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("Seller ID not found. Cannot update seller information in the database.");
                }
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("User property is null. Cannot update seller information.");
            }
        }

        private void NavigateToProfilePage()
        {
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
