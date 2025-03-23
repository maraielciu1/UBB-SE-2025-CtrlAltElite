using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using MarketPlace924.ViewModel;
using Microsoft.UI.Xaml;

namespace MarketPlace924.View
{
    public sealed partial class UpdateProfileView : Page
    {
        public UpdateProfileView()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (e.Parameter is SellerProfileViewModel viewModel)
            {
                DataContext = viewModel;
            }
        }
        private void SaveButtonClick(object sender, RoutedEventArgs e)
        {
            var viewModel = (SellerProfileViewModel)DataContext;
            viewModel.UpdateProfileCommand.Execute(null);
        }

        private void BackButtonClick(object sender, RoutedEventArgs e)
        {
            var frame = Window.Current.Content as Frame;
            if (frame != null && frame.CanGoBack)
            {
                frame.GoBack();
            }
            else
            {
                // Navigate back to the SellerProfileView if the frame cannot go back
                frame.Navigate(typeof(SellerProfileView));
            }
        }
    }
}