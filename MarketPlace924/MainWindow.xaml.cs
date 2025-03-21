using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using MarketPlace924.Repository;
using MarketPlace924.Service;
using MarketPlace924.View;
using MarketPlace924.DBConnection;
using MarketPlace924.Domain;
using MarketPlace924.ViewModel;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace MarketPlace924
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window, OnLoginSuccessCallback
    {
        private UserService _userService;
        private BuyerService _buyerService;
        private User? _user;

        public MainWindow()
        {
            this.InitializeComponent();

            // Initialize Database Connection and Services
            var dbConnection = new DBConnection.DBConnection(); // Using your DBConnection class
            var userRepo = new UserRepository(dbConnection);
            var buyerRepo = new BuyerRepository(dbConnection);

            _userService = new UserService(userRepo);
            _buyerService = new BuyerService(buyerRepo, userRepo);

            LoginView.ViewModel = new LoginViewModel(_userService, this);

            // TODO Navigate to Login
            // _user = new User(userID: 1, phoneNumber: "074322321", email: "admin@gmail.com");
            // NavigateToBuyerProfile();

            // Set the content of the window
        }

        public Visibility MenuAndStageVisibility => _user != null ? Visibility.Visible : Visibility.Collapsed;
        public Visibility LoginVisibility => _user == null ? Visibility.Visible : Visibility.Collapsed;

        public void OnLoginSuccess(User user)
        {
            LoginView.Visibility = Visibility.Collapsed;
            MenuAndStage.Visibility = Visibility.Visible;
            _user = user;
            if (user.Role == 1)
            {
                NavigateToBuyerProfile();
            }
        }

        private void NavigateToLogin()
        {
            Stage.Navigate(typeof(LoginView), new LoginViewModel(_userService, this));
        }
        
        private void NavigateToHome()
        {
            NavigateToLogin();
        }
        
        private void NavigateToMarketplace()
        {
            NavigateToLogin();
        }
        private void NavigateToBuyerProfile()
        {
            Stage.Navigate(typeof(BuyerProfileView), new BuyerProfileViewModel(_buyerService, _user));
        }
    }
}