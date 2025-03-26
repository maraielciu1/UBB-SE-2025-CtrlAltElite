using Microsoft.UI.Xaml;
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
            InitializeComponent();

            // Initialize Database Connection and Services
            var dbConnection = new DatabaseConnection(); // Using your DBConnection class
            var userRepo = new UserRepository(dbConnection);
            var buyerRepo = new BuyerRepository(dbConnection);

            // Initialize Services
            _userService = new UserService(userRepo);
            _buyerService = new BuyerService(buyerRepo, userRepo);

            LoginFrame.Navigate(typeof(LoginView), new LoginViewModel(_userService, this));
            // To Start Logged in as Buyer ucomment bellow
            // _user = new User(userID: 5, phoneNumber: "074322321", email: "admin@gmail.com");
            // MenuAndStage.Visibility = Visibility.Visible;
            // LoginView.Visibility = Visibility.Collapsed;
            // NavigateToBuyerProfile();
        }

        public void OnLoginSuccess(User user)
        {
            LoginFrame.Visibility = Visibility.Collapsed;
            MenuAndStage.Visibility = Visibility.Visible;
            _user = user;
            if (user.Role == UserRole.Buyer)
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
            Stage.Navigate(typeof(MyMarketView), new MyMarketViewModel(_buyerService, _user));
        }
        private void NavigateToBuyerProfile()
        {
            Stage.Navigate(typeof(BuyerProfileView), new BuyerProfileViewModel(_buyerService, _user, new BuyerWishlistItemDetailsProvider()));
        }
    }
}