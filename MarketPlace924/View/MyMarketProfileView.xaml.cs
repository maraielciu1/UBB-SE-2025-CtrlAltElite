using MarketPlace924.Domain;
using MarketPlace924.ViewModel;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

namespace MarketPlace924.View
{
    public sealed partial class MyMarketProfileView : Page
    {
        private Seller _selectedSeller;
        private MyMarketProfileViewModel _viewModel;

        public MyMarketProfileView()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            // Retrieve the seller from the navigation parameter
            _selectedSeller = e.Parameter as Seller;

            // Initialize the ViewModel with the selected seller
            _viewModel = new MyMarketProfileViewModel(_selectedSeller);
            this.DataContext = _viewModel;
        }

        private void GoBackToMyMarket(object sender, RoutedEventArgs e)
        {
            // Navigate back to the "MyMarket" page (or the previous page in the navigation stack)
            if (Frame.CanGoBack)
            {
                Frame.GoBack();
            }
        }
    }
}
