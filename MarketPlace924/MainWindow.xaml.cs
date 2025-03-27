using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using MarketPlace924.DBConnection;
using MarketPlace924.Domain;
using MarketPlace924.Repository;
using MarketPlace924.Service;
using MarketPlace924.View;
using MarketPlace924.View.Admin;
using MarketPlace924.ViewModel;
using MarketPlace924.ViewModel.Admin;
using Microsoft.UI.Xaml;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace MarketPlace924
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window,INotifyPropertyChanged, IOnLoginSuccessCallback
    {
        private UserService _userService;
        private BuyerService _buyerService;
        private SellerService _sellerService;
        private User? _user;
        private AdminService _adminService;
        private AnalyticsService _analyticsService;

        public MainWindow()
        {
            InitializeComponent();

            // Initialize Database Connection and Services
            var dbConnection = new DatabaseConnection(); // Using your DBConnection class
            var userRepo = new UserRepository(dbConnection);
            var buyerRepo = new BuyerRepository(dbConnection);
            var sellerRepo = new SellerRepository(dbConnection, userRepo);

            // Initialize Services
            _userService = new UserService(userRepo);
            _buyerService = new BuyerService(buyerRepo, userRepo);

            _adminService = new AdminService(userRepo);
            _analyticsService = new AnalyticsService(userRepo, buyerRepo);

            _sellerService = new SellerService(sellerRepo);

            LoginFrame.Navigate(typeof(LoginView), new LoginViewModel(_userService, this));
        }


        public bool IsUserLogedIn => _user != null;
        public bool IsMarketPlaceButtonVisible => IsUserLogedIn && _user!.Role == UserRole.Buyer;

        public bool IsProfileButtonVisibile =>
            IsUserLogedIn && (_user!.Role == UserRole.Buyer || _user.Role == UserRole.Seller);

        public Visibility SceneVibility => IsUserLogedIn ? Visibility.Visible : Visibility.Collapsed;
        public Visibility LoginVisibility => IsUserLogedIn ? Visibility.Collapsed : Visibility.Visible;

        public Visibility MarketPlaceButtonVisibility =>
            IsMarketPlaceButtonVisible ? Visibility.Visible : Visibility.Collapsed;

        public Visibility ProfileButtonVisibility =>
            IsProfileButtonVisibile ? Visibility.Visible : Visibility.Collapsed;
        
        public async Task OnLoginSuccess(User user)
        {
            _user = user;
            switch (user.Role)
            {
                case UserRole.Buyer:
                    await NavigateToBuyerProfile(); break;
                case UserRole.Seller:
                    NavigateToSellerProfile();
                    break;
                case UserRole.Admin:
                    NavigateToAdminProfile();
                    break;
            }
            OnPropertyChanged(nameof(LoginVisibility));
            OnPropertyChanged(nameof(SceneVibility));
            OnPropertyChanged(nameof(ProfileButtonVisibility));
            OnPropertyChanged(nameof(MarketPlaceButtonVisibility));
        }


        private void NavigateToLogin()
        {
            Stage.Navigate(typeof(LoginView), new LoginViewModel(_userService, this));
        }

        private void NavigateToHome()
        {
            NavigateToLogin();
        }

        private void NavigateToMyMarket()
        {
            Stage.Navigate(typeof(MyMarketView), new MyMarketViewModel(_buyerService, _user!));
        }

private void NavigateToSellerProfile()
        {
            Stage.Navigate(typeof(SellerProfileView), new SellerProfileViewModel(_user!, _userService, _sellerService));
        }

        private async Task NavigateToBuyerProfile()
        {
            var buyerVm = new BuyerProfileViewModel
            {
                BuyerService = _buyerService,
                User = _user!,
                WishlistItemDetailsProvider = new BuyerWishlistItemDetailsProvider()
            };
            await buyerVm.LoadBuyerProfile();
            Stage.Navigate(typeof(BuyerProfileView), buyerVm);
        }

        private void NavigateToAdminProfile()
        {
            Stage.Navigate(typeof(AdminView), new AdminViewModel(_adminService, _analyticsService, _userService));
        }

        private async Task NavigateToProfile()
        {
            if (_user == null) return;

            switch (_user.Role)
            {
                case UserRole.Buyer:
                    var buyerProfileVm = new BuyerProfileViewModel
                    {
                        BuyerService = _buyerService,
                        User = _user,
                        WishlistItemDetailsProvider = new BuyerWishlistItemDetailsProvider()
                    };
                    await buyerProfileVm.LoadBuyerProfile();
                    Stage.Navigate(typeof(BuyerProfileView), buyerProfileVm);
                    break;
                case UserRole.Seller:
                    Stage.Navigate(typeof(SellerProfileView), new SellerProfileViewModel(_user, _userService, _sellerService));
                    break;
                case UserRole.Admin:
                    Stage.Navigate(typeof(AdminView), new AdminViewModel(_adminService, _analyticsService, _userService));
                    break;
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        
    }
}