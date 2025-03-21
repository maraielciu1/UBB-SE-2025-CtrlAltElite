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
            Frame.Navigate(typeof(SignUpPage), ViewModel);
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
