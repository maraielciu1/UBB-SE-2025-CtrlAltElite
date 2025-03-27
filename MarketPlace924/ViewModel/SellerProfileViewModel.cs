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
using System.Collections.Generic;

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

        private bool _isExpanderExpanded = false;
        private bool _isNotificationsLoaded = false;

        public bool IsExpanderExpanded
        {
            get => _isExpanderExpanded;
            set
            {
                if (_isExpanderExpanded != value)
                {
                    _isExpanderExpanded = value;
                    OnPropertyChanged();
                    if (_isExpanderExpanded && !_isNotificationsLoaded)
                    {
                        // Load notifications only once when expander is expanded
                        LoadNotifications();
                        _isNotificationsLoaded = true;
                    }
                }
            }
        }


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

        public bool CreationMode { get; set; }

        private void LoadSellerProfile()
        {
            CreationMode = _seller.StoreName == null;

            if (_seller != null)
            {
                StoreName = _seller.StoreName;
                Username = _seller.Username;
                Email = _seller.Email;
                PhoneNumber = _seller.PhoneNumber;
                Address = _seller.StoreAddress;
                FollowersCount = _seller.FollowersCount.ToString();
                TrustScore = _seller.TrustScore * 100.0 / 5.0;
                //TrustScore = _sellerService.CalculateAverageReviewScore(_seller.Id) * 100.0 / 5.0;
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
            //if (CreationMode)
            //{
            //    //Navigate to the updateProfile page
            //    UpdateProfile();
            //}
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
            if (CreationMode)
            {
                await _sellerService.CreateSeller(_seller);

            }
            else
            {
                await _sellerService.UpdateSellerAsync(_seller);
            }

            if (_seller != null)
            {
                _seller.StoreName = StoreName;
                //_seller.Email = Email;
                //_seller.PhoneNumber = PhoneNumber;
                _seller.StoreAddress = Address;
                _seller.StoreDescription = Description;

                if (_seller.Id > 0)
                {
                    await _sellerService.UpdateSellerAsync(_seller);
                    await ShowDialog("Success", "Your seller has been updated successfully!");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("Seller ID not found. Cannot update seller information in the database.");
                    await ShowDialog("Error", "Seller ID not found. Cannot update seller information in the database.");
                }
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("User property is null. Cannot update seller information.");
            }
        }

        private async System.Threading.Tasks.Task ShowDialog(string title, string message)
        {
            ContentDialog dialog = new ContentDialog
            {
                Title = title,
                Content = message,
                CloseButtonText = "OK",
                XamlRoot = App.m_window.Content.XamlRoot
            };

            await dialog.ShowAsync();
        }

        public void SortProducts()
        {
            var sortedProducts = _allProducts.OrderBy(p => p.Price).ToList();
            FilteredProducts = new ObservableCollection<Product>(sortedProducts);
            OnPropertyChanged(nameof(FilteredProducts));
        }

        public string StoreNameError { get; set; }
        public string EmailError { get; set; }
        public string PhoneNumberError { get; set; }
        public string AddressError { get; set; }
        public string DescriptionError { get; set; }

        public List<string> ValidateFields()
        {
            List<string> errorMessages = new List<string>();

            if (string.IsNullOrWhiteSpace(StoreName))
            {
                StoreNameError = "Store name is required.";
                errorMessages.Add(StoreNameError);
            }
            else
            {
                StoreNameError = string.Empty;
            }

            if (string.IsNullOrWhiteSpace(Email) || !Email.Contains("@"))
            {
                EmailError = "Valid email is required.";
                errorMessages.Add(EmailError);
            }
            else
            {
                EmailError = string.Empty;
            }

            if (string.IsNullOrWhiteSpace(PhoneNumber))
            {
                PhoneNumberError = "Phone number is required.";
                errorMessages.Add(PhoneNumberError);
            }
            else
            {
                PhoneNumberError = string.Empty;
            }

            if (string.IsNullOrWhiteSpace(Address))
            {
                AddressError = "Address is required.";
                errorMessages.Add(AddressError);
            }
            else
            {
                AddressError = string.Empty;
            }

            if (string.IsNullOrWhiteSpace(Description))
            {
                DescriptionError = "Description is required.";
                errorMessages.Add(DescriptionError);
            }
            else
            {
                DescriptionError = string.Empty;
            }

            OnPropertyChanged(nameof(StoreNameError));
            OnPropertyChanged(nameof(EmailError));
            OnPropertyChanged(nameof(PhoneNumberError));
            OnPropertyChanged(nameof(AddressError));
            OnPropertyChanged(nameof(DescriptionError));

            return errorMessages;
        }

        public async Task LoadNotifications()
        {
            var notifications = await _sellerService.GetNotifications(_seller.Id, _seller.FollowersCount);
            _notifications.Clear();
            foreach (var notification in notifications)
            {
                _notifications.Add(notification);
            }
            OnPropertyChanged(nameof(Notifications));
        }


        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
