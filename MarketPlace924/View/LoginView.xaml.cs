using MarketPlace924.Repository;
using MarketPlace924.Service;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;

namespace MarketPlace924.View
{
    public sealed partial class LoginView : Page
    {
        private readonly LoginViewModel _viewModel;

        public LoginView()
        {
            this.InitializeComponent();

            var dbConnection = new DBConnection.DatabaseConnection();
            var userRepository = new UserRepository(dbConnection);
            var userService = new UserService(userRepository);
            _viewModel = new LoginViewModel(userService);

            DataContext = _viewModel;
        }
        private void RegisterButtonTextBlock_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            Frame.Navigate(typeof(SignUpPage), _viewModel._userService);
        }
    }
}
