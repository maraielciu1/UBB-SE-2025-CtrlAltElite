using MarketPlace924.Domain;
using MarketPlace924.ViewModel;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

namespace MarketPlace924.View
{
    public sealed partial class MyMarketProfileView : Page
    {
        private MyMarketProfileViewModel? _viewModel;
        public MyMarketProfileViewModel ViewModel => _viewModel;

        public MyMarketProfileView()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            // If the navigation parameter contains a MyMarketProfileViewModel, assign it to _viewModel
            if (e.Parameter is MyMarketProfileViewModel viewModel)
            {
                _viewModel = viewModel;
                DataContext = _viewModel;
            }
        }

        private void GoBackToMyMarket(object sender, RoutedEventArgs e)
        {
            // Navigate back to the "MyMarket" page (or the previous page in the navigation stack)
            if (Frame.CanGoBack)
            {
                Frame.GoBack();
            }
        }

        // Handles the text change event in the product search text box.
        private void OnSearchProductTextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (_viewModel != null && textBox != null)
            {
                _viewModel.FilterProducts(textBox.Text); // Pass the search text to the ViewModel
            }
        }
    }
}
