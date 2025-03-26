using MarketPlace924.ViewModel;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Navigation;

namespace MarketPlace924.View
{
    public sealed partial class LoginView : Page
    {

        public LoginView()
        {

            InitializeComponent();
        }
        
        public LoginViewModel ViewModel
        {
            get => (LoginViewModel)DataContext;
            set => DataContext = value;
        }

        private void RegisterButtonTextBlock_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            var signUpViewModel = new SignUpViewModel(ViewModel.UserService);
            signUpViewModel.NavigateToLogin = () => {
                Frame.Navigate(typeof(LoginView), ViewModel);
            };
            Frame.Navigate(typeof(SignUpPage), signUpViewModel);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (e.Parameter is LoginViewModel viewModel)
            {
                ViewModel = viewModel;
            }
            
        }
    }


}
