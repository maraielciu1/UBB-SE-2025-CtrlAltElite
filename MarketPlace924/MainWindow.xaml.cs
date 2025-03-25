using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using MarketPlace924.Domain;
using MarketPlace924.Repository;
using MarketPlace924.Service;
using MarketPlace924.View;
using MarketPlace924.DBConnection;
using MarketPlace924.ViewModel;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace MarketPlace924
{
	/// <summary>
	/// An empty window that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class MainWindow : Window
    {
		public static MainWindow _instance { get; set; }
        private UserService _userService;
        private BuyerService _buyerService;
        private User? _user;


        // Controls the visibility of the menu and content stage.
        public Visibility MenuAndStageVisibility => _user != null ? Visibility.Visible : Visibility.Collapsed;

        // Controls the visibility of the login section.
        public Visibility LoginVisibility => _user == null ? Visibility.Visible : Visibility.Collapsed;


        public MainWindow()
        {
            InitializeComponent();
			_instance = this;

            // Initialize Database Connection and Services
            var dbConnection = new DatabaseConnection(); // Using your DBConnection class
            var userRepo = new UserRepository(dbConnection);
            var buyerRepo = new BuyerRepository(dbConnection);

            // Initialize Services
            _userService = new UserService(userRepo);
            _buyerService = new BuyerService(buyerRepo, userRepo);

            // Temporary hardcoded user for testing purposes
            _user = new User(userID: 3, phoneNumber: "074322321", email: "admin@gmail.com");

            // Ensure menu and content area is visible
            MenuAndStage.Visibility = Visibility.Visible;

            // Navigate to the Buyer's Profile page
            NavigateToBuyerProfile();
        }

        private void NavigateToBuyerProfile()
        {
            Stage.Navigate(typeof(MyMarketView), new MyMarketViewModel(_buyerService, _user));
        }
    }
}
