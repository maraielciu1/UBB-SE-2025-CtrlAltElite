﻿using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using MarketPlace924.Service;

namespace MarketPlace924.ViewModel
{
    public class SellerProfileViewModel : INotifyPropertyChanged
    {
        private UserService _userService;
        private SellerService _sellerService;
        private string _username;

        public SellerProfileViewModel(UserService userService, SellerService sellerService, string username)
        {
            _userService = userService;
            _sellerService = sellerService;
            _username = username;
            LoadSellerProfile();
        }

        public string DisplayName { get; set; }
        public string Name { get; set; }
        public string FollowersCount { get; set; }
        public string StoreName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public double TrustScore { get; set; }
        public ICommand UpdateProfileCommand { get; set; }



        private void LoadSellerProfile()
        {
            var currentSeller = _sellerService.GetSeller(_username);
            if (currentSeller != null)
            {
                DisplayName = currentSeller.StoreName;
                StoreName = currentSeller.Username;
                Email = currentSeller.Email;
                PhoneNumber = currentSeller.PhoneNumber;
                Address = currentSeller.StoreAddress;
                FollowersCount = currentSeller.FollowersCount.ToString();
                TrustScore = currentSeller.TrustScore;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
