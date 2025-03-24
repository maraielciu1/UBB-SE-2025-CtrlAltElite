using Microsoft.UI.Xaml.Controls;
using MarketPlace924.Service;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Navigation;
using MarketPlace924.ViewModel;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace MarketPlace924.View
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SellerProfileView : Page
    {
        public SellerProfileView()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (e.Parameter is object[] parameters && parameters.Length == 2)
            {
                var userService = parameters[0] as UserService;
                var sellerService = parameters[1] as SellerService;
                DataContext = new SellerProfileViewModel(userService, sellerService, "alice_smith");
            }
        }

        private void OnSearchTextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = sender as TextBox;
            var viewModel = DataContext as SellerProfileViewModel;
            if (viewModel != null && textBox != null)
            {
                viewModel.FilterProducts(textBox.Text);
            }
        }

        private void OnUpdateProfileButtonClick(object sender, RoutedEventArgs e)
        {
            var viewModel = DataContext as SellerProfileViewModel;
            if (viewModel != null)
            {
                Frame.Navigate(typeof(UpdateProfileView), viewModel);
            }
        }
        //private void OnProductItemClick(object sender, ItemClickEventArgs e)
        //{
        //    var selectedProduct = e.ClickedItem as ProductViewModel;
        //    if (selectedProduct != null)
        //    {
        //        // Implement the logic to delete the selected product
        //        // For example, you can call a method in the ViewModel to remove the product from the collection
        //        var viewModel = this.DataContext as SellerProfileViewModel;
        //        viewModel?.DeleteProductCommand.Execute(selectedProduct);
        //    }
        //}
    }
}
