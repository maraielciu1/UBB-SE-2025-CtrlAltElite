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

namespace MarketPlace924.ViewModel
{
    public class MyMarketProfileViewModel : INotifyPropertyChanged
    {
        // Private fields to store seller and service information
        private Seller _seller;
        private BuyerService _buyerService;


        // ObservableCollection to store all products and filtered products for display
        private ObservableCollection<Product> _allProducts;
        private ObservableCollection<Product> _filteredProducts;

        // Public property for filtered products bound to the view
        public ObservableCollection<Product> FilteredProducts { get; set; }
        public ObservableCollection<Product> Products { get; set; } = new ObservableCollection<Product>();


        // Public properties for binding to the view
        public string DisplayName { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string FollowersCount { get; set; } = string.Empty;
        public string StoreName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public double TrustScore { get; set; }
        public string Description { get; set; } = string.Empty;


        public MyMarketProfileViewModel(Seller seller)
        {
            _seller = seller;
            _allProducts = new ObservableCollection<Product>();
            FilteredProducts = new ObservableCollection<Product>();
            LoadSellerProfile(); // Load seller's profile data
        }
 

        private async void LoadSellerProfile()
        {
            if (_seller != null)
            {
                StoreName = _seller.StoreName;
                //Username = _seller.Username;
                //Email = _seller.Email;
                //PhoneNumber = _seller.PhoneNumber;
                Address = _seller.StoreAddress;
                FollowersCount = _seller.FollowersCount.ToString();
                TrustScore = _seller.TrustScore;
                Description = _seller.StoreDescription;


                // Notify the view of property changes to update the UI
                OnPropertyChanged(nameof(DisplayName));
                OnPropertyChanged(nameof(StoreName));
                //OnPropertyChanged(nameof(Email));
                //OnPropertyChanged(nameof(PhoneNumber));
                OnPropertyChanged(nameof(Address));
                OnPropertyChanged(nameof(Description));
                OnPropertyChanged(nameof(FollowersCount));
                OnPropertyChanged(nameof(TrustScore));
            }
        }


        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
